﻿using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using Bitbird.Core.Extensions;
using Bitbird.Core.Json.Extensions;
using Bitbird.Core.WebApi.Net.Models;
using Newtonsoft.Json.Serialization;

namespace Bitbird.Core.WebApi.Net
{
    public static class JsonApiErrorExtensions
    {
        public static bool LogDetailledMessages = false;

        public static HttpResponseMessage ToJsonApiErrorResponseMessage(this Exception exc)
        {
            var jsonApiErrors = exc.ToJsonApiErrors();
            var formatter = new JsonMediaTypeFormatter
            {
                SerializerSettings =
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            };

            return new HttpResponseMessage((HttpStatusCode)int.Parse(jsonApiErrors.Errors.FirstOrDefault()?.Status ?? ((int)HttpStatusCode.BadRequest).ToString()))
            {
                Content = new ObjectContent<JsonApiErrors>(jsonApiErrors, formatter)
            };
        }
        public static JsonApiErrors ToJsonApiErrors(this Exception exc)
        {
            return new JsonApiErrors
            {
                Errors = exc.ToJsonApiErrorObjects()
            };
        }
        public static JsonApiErrorObject[] ToJsonApiErrorObjects(this Exception exc)
        {
            return exc.GetAggregatedExceptions().SelectMany(aggExc =>
            {
                switch (aggExc)
                {
                    case ApiErrorException aee:
                        return aee.ToJsonApiErrorObjects();
                }

                return new[]
                {
                    new JsonApiErrorObject
                    {
                        Id = Guid.NewGuid().ToString(),
                        Code = ((int) HttpStatusCode.InternalServerError).ToString(),
                        Detail = LogDetailledMessages ? aggExc.ToString() : "Internal Server Error",
                        Status = ((int) HttpStatusCode.InternalServerError).ToString(),
                        Title = "Internal Server Error",
                        Source = new JsonApiErrorSource
                        {
                            Pointer = "/data",
                            Parameter = ""
                        },
                        Links = new JsonApiErrorLinks
                        {
                            About = ""
                        }
                    }
                };
            }).ToArray();
        }
        public static JsonApiErrorObject[] ToJsonApiErrorObjects(this ApiErrorException exc)
        {
            return exc.ApiErrors.Select(apiError =>
            {
                switch (apiError)
                {
                    case ApiAttributeError attributeError:
                        return new JsonApiErrorObject
                        {
                            Id = Guid.NewGuid().ToString(),
                            Code = ((int)apiError.Type).ToString(),
                            Title = apiError.Title,
                            Detail = apiError.DetailMessage,
                            Status = typeof(ApiErrorType).GetMember(apiError.Type.ToString())?.FirstOrDefault()
                                         ?.GetCustomAttributes<HttpStatusCodeAttribute>().FirstOrDefault()?.StatusCode
                                         .ToString() ?? ((int)HttpStatusCode.InternalServerError).ToString(),
                            Source = new JsonApiErrorSource
                            {
                                Pointer = $"/data/attributes/{attributeError.AttributeName.FromCamelCaseToJsonCamelCase()}",
                                Parameter = ""
                            },
                            Links = new JsonApiErrorLinks
                            {
                                About = ""
                            }
                        };
                    case ApiParameterError parameterError:
                        return new JsonApiErrorObject
                        {
                            Id = Guid.NewGuid().ToString(),
                            Code = ((int)apiError.Type).ToString(),
                            Title = apiError.Title,
                            Detail = apiError.DetailMessage,
                            Status = typeof(ApiErrorType).GetMember(apiError.Type.ToString())?.FirstOrDefault()
                                         ?.GetCustomAttributes<HttpStatusCodeAttribute>().FirstOrDefault()?.StatusCode
                                         .ToString() ?? ((int)HttpStatusCode.InternalServerError).ToString(),
                            Source = new JsonApiErrorSource
                            {
                                Pointer = $"/data/attributes/{parameterError.ParameterName.FromCamelCaseToJsonCamelCase()}",
                                Parameter = ""
                            },
                            Links = new JsonApiErrorLinks
                            {
                                About = ""
                            }
                        };
                    default:
                        return new JsonApiErrorObject
                        {
                            Id = Guid.NewGuid().ToString(),
                            Code = ((int)apiError.Type).ToString(),
                            Title = apiError.Title,
                            Detail = apiError.DetailMessage,
                            Status = typeof(ApiErrorType).GetMember(apiError.Type.ToString())?.FirstOrDefault()
                                         ?.GetCustomAttributes<HttpStatusCodeAttribute>().FirstOrDefault()?.StatusCode
                                         .ToString() ?? ((int)HttpStatusCode.InternalServerError).ToString(),
                            Source = new JsonApiErrorSource
                            {
                                Pointer = "/data",
                                Parameter = ""
                            },
                            Links = new JsonApiErrorLinks
                            {
                                About = ""
                            }
                        };
                }
            }).ToArray();
        }
    }
}