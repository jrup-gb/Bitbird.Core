﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Bitbird.Core.Data;
using Bitbird.Core.Data.CliToolAnnotations;
using Bitbird.Core.Data.DbContext;
using Bitbird.Core.Data.Validation;
using Bitbird.Core.Types;
using JetBrains.Annotations;

namespace Bitbird.Core.Ide.Tools.Api.CliTools
{
    [UsedImplicitly]
    public sealed class DataModelReader
    {
        [NotNull] private readonly string modelPostfix;
        [NotNull, ItemNotNull] private readonly Type[] dataModelTypes;

        [UsedImplicitly]
        public DataModelReader(
            [NotNull] string modelPostfix, 
            [NotNull] Assembly dataModelTypeAssembly)
        {
            this.modelPostfix = modelPostfix ?? throw new ArgumentNullException(nameof(modelPostfix));

            dataModelTypes = (dataModelTypeAssembly ?? throw new ArgumentNullException(nameof(dataModelTypeAssembly)))
                .GetTypes()
                .Where(t => t.GetCustomAttribute<EntityAttribute>() != null)
                .ToArray();
        }

        [NotNull, UsedImplicitly]
        public DataModelAssemblyInfo ExtractDataModelInfo()
        {
            var dataModelInfos = dataModelTypes
                .Select(ExtractDataModelInfo)
                .ToArray();

            return new DataModelAssemblyInfo(dataModelInfos);
        }

        [NotNull]
        private DataModelInfo ExtractDataModelInfo([NotNull] Type dataModelType)
        {
            if (dataModelType == null) throw new ArgumentNullException(nameof(dataModelType));

            if (!dataModelType.Name.EndsWith(modelPostfix))
                throw new Exception($"Class {dataModelType.FullName} does not have the postfix {modelPostfix}.");

            var tableAttribute = dataModelType.GetCustomAttribute<TableAttribute>()
                ?? throw new Exception($"Class {dataModelType.FullName} as no {nameof(TableAttribute)} defined.");
            if (tableAttribute.Name == null)
                throw new Exception($"Class {dataModelType.FullName} as a {nameof(TableAttribute)} with {nameof(tableAttribute.Name)} = null.");

            var isIIsDeletedFlagEntity = typeof(IIsDeletedFlagEntity).IsAssignableFrom(dataModelType);
            var isIIsLockedFlagEntity = typeof(IIsLockedFlagEntity).IsAssignableFrom(dataModelType);
            var isIIsActiveFlagEntity = typeof(IIsActiveFlagEntity).IsAssignableFrom(dataModelType);
            var isIOptimisticLockable = typeof(IOptimisticLockable).IsAssignableFrom(dataModelType);
            var idType = dataModelType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IId<>))?.GetGenericArguments()[0];
            var idSetterType = dataModelType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IIdSetter<>))?.GetGenericArguments()[0];
            var importKeyType = dataModelType.GetProperties().SingleOrDefault(p => p.Name.Equals("ImportKey"))?.PropertyType;
            var importKeyUnderlyingType = importKeyType != null
                    ? (Nullable.GetUnderlyingType(importKeyType) ?? importKeyType)
                    : null;

            var baseType = dataModelType;
            var isTemporalTableBase = false;
            while (baseType != null)
            {
                if (baseType.Name.Equals("TemporalTableDbModelBase"))
                {
                    isTemporalTableBase = true;
                    break;
                }

                baseType = baseType.BaseType;
            }

            return new DataModelInfo(
                dataModelType.Name,
                tableAttribute.Name,
                dataModelType.Name.Substring(0, dataModelType.Name.Length - modelPostfix.Length),
                dataModelType.GetProperties()
                    .Select(ExtractDataModelPropertyInfo)
                    .Where(x => x != null)
                    .ToArray(),
                isIIsDeletedFlagEntity,
                isIIsLockedFlagEntity,
                isIIsActiveFlagEntity,
                isIOptimisticLockable,
                isTemporalTableBase,
                idType?.ToCsType(),
                idSetterType?.ToCsType(),
                importKeyType?.ToCsType(),
                importKeyUnderlyingType?.ToCsType());
        }

        [CanBeNull]
        private DataModelPropertyInfo ExtractDataModelPropertyInfo([NotNull] PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) throw new ArgumentNullException(nameof(propertyInfo));

            if (propertyInfo.GetCustomAttribute<IgnoreInApiModelAttribute>() != null)
                return null;

            var isNavigationalProperty = propertyInfo.GetGetMethod().IsVirtual && !propertyInfo.GetGetMethod().IsFinal;
            if (isNavigationalProperty)
                return null;

            var type = propertyInfo.PropertyType;

            var isSqlNullable = (!type.IsValueType || Nullable.GetUnderlyingType(type) != null) && type.GetCustomAttribute<RequiredAttribute>() == null;

            var isKey = propertyInfo.GetCustomAttribute<KeyAttribute>() != null;
            var dbGeneratedAttribute = propertyInfo.GetCustomAttribute<DatabaseGeneratedAttribute>();
            var isDbGenerated = dbGeneratedAttribute != null;
            var isIdentity = dbGeneratedAttribute?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;
            var isComputed = dbGeneratedAttribute?.DatabaseGeneratedOption == DatabaseGeneratedOption.Computed;

            var foreignKeyNavigationalPropertyInfo = propertyInfo.DeclaringType?.GetProperties().FirstOrDefault(p =>
                p.GetCustomAttribute<ForeignKeyAttribute>()?.Name?.Equals(propertyInfo.Name) ?? false);
            var foreignKeyDataModelClass = foreignKeyNavigationalPropertyInfo?.PropertyType;
            var foreignKeyDataModelClassName = foreignKeyDataModelClass?.Name;
            var foreignKeyDataModelModelName = foreignKeyDataModelClassName?.Substring(0, foreignKeyDataModelClassName.Length - modelPostfix.Length);

            var stringLengthAttribute = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
            var stringInfo = type != typeof(string)
                ? null
                : new DataModelPropertyStringInfo(stringLengthAttribute?.MaximumLength);

            var persistInApiModel = !(typeof(IOptimisticLockable).IsAssignableFrom(propertyInfo.DeclaringType) &&
                propertyInfo.Name.Equals(nameof(IOptimisticLockable.SysStartTime)));

            var nullableGenericUnderlyingType = Nullable.GetUnderlyingType(type);
            var isNullableGeneric = nullableGenericUnderlyingType != null;
            nullableGenericUnderlyingType = nullableGenericUnderlyingType ?? type;
            var canTypeBeAssignedNull = type.IsClass || isNullableGeneric;

            var isNotMapped = propertyInfo.GetCustomAttribute<NotMappedAttribute>() != null;

            return new DataModelPropertyInfo(
                propertyInfo.Name,
                type.ToCsType(),
                isSqlNullable,
                isNullableGeneric,
                nullableGenericUnderlyingType.ToCsType(),
                nullableGenericUnderlyingType.IsEnum,
                canTypeBeAssignedNull,
                isKey,
                isDbGenerated,
                isIdentity,
                isComputed,
                isNotMapped,
                persistInApiModel,
                foreignKeyDataModelClass?.ToCsType(),
                foreignKeyDataModelClassName,
                foreignKeyDataModelModelName,
                stringInfo,
                propertyInfo.GetCustomAttributes(true)
                    .Select(ExtractDataModelValidationAttributeInfo)
                    .Where(a => a != null)
                    .ToArray());
        }

        [CanBeNull]
        private static Attribute ExtractDataModelValidationAttributeInfo([NotNull] object attribute)
        {
            if (attribute == null) throw new ArgumentNullException(nameof(attribute));

            if (!(attribute is Attribute))
                return null;

            AttributeTranslations.TryTranslateAttribute(attribute, out attribute);

            var type = attribute.GetType();

            // ReSharper disable once PossibleNullReferenceException
            if (!type.FullName.StartsWith("Bitbird.Core.Data.Validation"))
                return null;
            
            return attribute as Attribute;
        }
    }
}
