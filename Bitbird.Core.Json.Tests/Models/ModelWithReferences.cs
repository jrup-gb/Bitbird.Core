﻿using Bitbird.Core.Json.JsonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitbird.Core.Json.Tests.Models
{
    public class ModelWithReferences : JsonApiBaseModel
    {
        public string Name { get; set; }

        public ModelWithNoReferences SingleReference { get; set; }

        public IEnumerable<ModelWithNoReferences> CollectionReference { get; set; }
        
    }
}
