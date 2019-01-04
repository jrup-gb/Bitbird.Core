﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;

namespace Bitbird.Core.Data.Net
{
    public class RedisCache
    {
        private readonly string connectionString;
        private readonly IContractResolver contractResolver;
        private readonly Lazy<ConnectionMultiplexer> lazyConnection;

        public RedisCache(string connectionString, IContractResolver contractResolver = null)
        {
            this.connectionString = connectionString;
            this.contractResolver = contractResolver;
            lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(this.connectionString));

            Clear();
        }

        public RedisCacheInfo GetInfo()
        {
            var ret = new RedisCacheInfo
            {
                IsConnected = IsConnected
            };
            if (!ret.IsConnected)
                return ret;

            var result = Db.Execute("INFO", "memory");
            if (!result.IsNull)
            {
                var str = result.ToString();
                {
                    Regex r = new Regex(@"used_memory_dataset:(\d*)?");
                    var m = r.Match(str);
                    if (m.Success)
                    {
                        if (m.Groups.Count > 1 && long.TryParse(m.Groups[1].Value, out var size))
                            ret.UsedMemory = size;
                    }
                }
                {
                    Regex r = new Regex(@"maxmemory:(\d*)?");
                    if (r.IsMatch(str))
                    {
                        var m = r.Match(str);
                        if (m.Groups.Count > 1 && long.TryParse(m.Groups[1].Value, out var size))
                            ret.MaximumMemory = size;
                    }
                }
            }
            return ret;
        }

        private ConnectionMultiplexer Connection => lazyConnection.Value;

        private IDatabase Db => Connection.GetDatabase();

        private bool IsConnected => Connection.IsConnected;

        public bool Clear()
        {
            if (!IsConnected) return false;
            Db.Execute("FLUSHALL");
            return true;

        }

        public long DeleteAll(string prefix)
        {
            var server = Connection.GetServer(connectionString.Split(',')[0]);
            if (!server.IsConnected) return -1;

            var keys = server.Keys(Db.Database, pattern: GetKey(prefix, "*").ToString()).ToArray();
            if (!IsConnected) return -1;
            var deletedItems = Db.KeyDelete(keys);
            return deletedItems;
        }

        public bool Delete<TKey>(string prefix, TKey id)
        {
            if (!IsConnected) return false;
            var key = GetKey(prefix, id);
            return !Db.KeyExists(key) || Db.KeyDelete(key);
        }
        public long Delete<TKey>(string prefix, TKey[] ids) => Delete(prefix, ids.AsEnumerable());
        public long Delete<TKey>(string prefix, IEnumerable<TKey> ids)
        {
            if (!IsConnected) return 0;
            var keys = new List<RedisKey>();

            foreach (var id in ids)
            {
                keys.Add(GetKey(prefix, id));
            }
            return Db.KeyDelete(keys.ToArray());
        }

        private static RedisKey GetKey<TKey>(string prefix, TKey id)
        {
            return $"{prefix}:{id}";
        }
        private static IEnumerable<KeyValuePair<TKey, RedisKey>> GetKeys<TKey>(string prefix, IEnumerable<TKey> ids)
        {
            return ids.Select(id => new KeyValuePair<TKey, RedisKey>(id, $"{prefix}:{id}"));
        }

        private bool Set<TKey, T>(string prefix, TKey id, T item)
        {
            var key = GetKey(prefix, id);
            return Set(key, item);
        }

        private bool Set<T>(RedisKey key, T item)
        {
            if (!IsConnected) return false;
            var serializedItem = SerializeObject(item);
            return Db.StringSet(key, serializedItem);
        }

        private bool SetMany<TKey, T>(string prefix, IDictionary<TKey, T> items)
        {
            if (!IsConnected) return false;
            var list = new List<KeyValuePair<RedisKey, RedisValue>>();
            foreach (var item in items)
            {
                list.Add(new KeyValuePair<RedisKey, RedisValue>(GetKey(prefix, item.Key), SerializeObject(item.Value)));
            }
            return Db.StringSet(list.ToArray());
        }

        public void AddOrUpdate<TKey, T>(string prefix, TKey id, T item)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            if (!IsConnected) return;

            var key = GetKey(prefix, id);
            var storedVal = SerializeObject(item);
            Db.StringSet(key, storedVal);
        }

        public async Task AddOrUpdate<TKey, T>(string prefix, TKey id, Func<TKey, Task<T>> valueFactory)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            if (!IsConnected) return;

            var key = GetKey(prefix, id);
            var storedVal = SerializeObject(await valueFactory(id));
            Db.StringSet(key, storedVal);
        }

        public T GetOrAdd<TKey, T>(string prefix, TKey id, Func<TKey, T> valueFactory)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            if (!IsConnected) return valueFactory(id);

            var key = GetKey(prefix, id);

            var storedVal = Db.StringGet(key);
            if (storedVal.HasValue)
            {
                try
                {
                    var ret = DeserializeObject<T>(storedVal);
                    return ret;
                }
                catch
                {
                    // ignored - cannot deserialize - must be refreshed
                }
            }

            var newVal = valueFactory(id);
            Set(prefix, id, newVal);
            return newVal;
        }

        public async Task<T> GetOrAdd<TKey, T>(string prefix, TKey id, Func<TKey, Task<T>> valueFactory)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            if (!IsConnected) return await valueFactory(id);

            var key = GetKey(prefix, id);

            var storedVal = Db.StringGet(key);
            if (storedVal.HasValue)
            {
                try
                {
                    var ret = DeserializeObject<T>(storedVal);
                    return ret;
                }
                catch
                {
                    // ignored - cannot deserialize - must be refreshed
                }
            }

            var newVal = await valueFactory(id);
            Set(prefix, id, newVal);
            return newVal;
        }

        public async Task<ICollection<T>> GetOrAddMany<TKey, T>(string prefix, IEnumerable<TKey> ids, Func<IEnumerable<TKey>, Task<IDictionary<TKey, T>>> valueFactory)
        {
            var ret = new List<T>();
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            if (!IsConnected)
            {
                return (await valueFactory(ids)).Values;
            }

            var keys = GetKeys(prefix, ids).ToDictionary(_ => _.Key, _ => _.Value);
            var missingIds = new List<TKey>();
            var storedVals = Db.StringGet(keys.Values.ToArray());
            for (int i = 0; i < storedVals.Length; i++)
            {
                var storedVal = storedVals[i];
                if (storedVal.HasValue)
                {
                    T element;
                    try
                    {
                        element = DeserializeObject<T>(storedVal);
                    }
                    catch
                    {
                        // ignored - cannot deserialize - must be refreshed
                        missingIds.Add(keys.ElementAt(i).Key);
                        break;
                    }
                    if (element != null && !element.Equals(default(T)))
                        ret.Add(element);
                    else
                        missingIds.Add(keys.ElementAt(i).Key);
                }
                else
                    missingIds.Add(keys.ElementAt(i).Key);
            }
            if (missingIds.Any())
            {
                var missingElements = await valueFactory(missingIds);
                ret.AddRange(missingElements.Values);
                SetMany(prefix, missingElements);
            }
            return ret;
        }

        private JsonSerializerSettings serializerSettings;

        private JsonSerializerSettings SerializerSettings =>
            serializerSettings ?? (serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.All,
                ContractResolver = contractResolver
            });
        private string SerializeObject(object objectToCache)
        {
            return JsonConvert.SerializeObject(objectToCache
                , Formatting.Indented
                , SerializerSettings);
        }
        private T DeserializeObject<T>(string serializedObject)
        {
            return JsonConvert.DeserializeObject<T>(serializedObject
                , SerializerSettings);
        }
    }
}