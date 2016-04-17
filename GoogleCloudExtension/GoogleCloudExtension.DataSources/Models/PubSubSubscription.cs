// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using Newtonsoft.Json;

namespace GoogleCloudExtension.DataSources.Models
{
    public class PubSubSubscription
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("topic")]
        public string Topic { get; set; }

        [JsonProperty("pushConfig")]
        public PubSubPushConfig PushConfig { get; set; }

        [JsonProperty("ackDeadlineSeconds")]
        public int AckDeadlineSeconds { get; set; }    
    }
}