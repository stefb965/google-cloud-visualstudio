// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace GoogleCloudExtension.DataSources.Models
{
    public class PubSubPushConfig
    {
        [JsonProperty("pushEndpoint")]
        public string PushEndpoint { get; set; }

        [JsonProperty("attributes")]
        public IDictionary<string, object> Attributes { get; set; }
    }
}