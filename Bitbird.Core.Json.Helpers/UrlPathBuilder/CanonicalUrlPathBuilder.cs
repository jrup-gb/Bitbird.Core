﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bitbird.Core.Json.Helpers.ApiResource.UrlPathBuilder
{
    /// <summary>
    /// Always builds canonical self paths, rather than relationship paths.
    /// </summary>
    public class CanonicalUrlPathBuilder : DefaultUrlPathBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalUrlPathBuilder"/> class.
        /// </summary>
        public CanonicalUrlPathBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanonicalUrlPathBuilder"/> class.
        /// </summary>
        /// <param name="prefix">A prefix for all urls generated by this instance of the CanonicalUrlPathBuilder class.</param>
        public CanonicalUrlPathBuilder(string prefix)
            : base(prefix)
        {
        }

        /// <summary>
        /// Returns a path in the form `/relatedResource.UrlPath/relatedResource.Id/`.
        /// </summary>
        /// <param name="resource">The resource this path is related to.</param>
        /// <param name="id">The unique id of the resource.</param>
        /// <param name="relationship">The relationship this path refers to.</param>
        /// <param name="relatedResourceId">The id of the related resource.</param>
        /// <returns>A <see cref="string"/> containing the path.</returns>
        public override string BuildRelationshipPath(
            JsonApiResource resource,
            string id,
            ResourceRelationship relationship,
            string relatedResourceId)
        {
            // empty if no id, because e.g. /api/people != /api/companies/1/employees
            // (all people is not the same as all employees for a company)
            return string.IsNullOrEmpty(relatedResourceId)
                ? null
                : BuildCanonicalPath(relationship.RelatedResource, relatedResourceId);
        }
    }
}
