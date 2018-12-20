﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Bitbird.Core.Json.JsonApi.UrlBuilder
{
    /// <summary>
    /// Interface for building UrlBuilders´.
    /// </summary>
    public interface IUrlPathBuilder
    {
        // collection of resources, e.g. /people/
        string BuildCanonicalPath(Type resources);

        // individual resource, e.g. /people/1/
        string BuildCanonicalPath(IJsonApiDataModel resource);

        // related resource, e.g. /people/1/employer
        string BuildRelationshipPath(IJsonApiDataModel resource, PropertyInfo relatedProperty);
    }

}
