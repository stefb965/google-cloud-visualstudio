// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoogleCloudExtension.DataSources.Models
{
    public class PubSubSubscriptionPage
    {
        [JsonProperty("subscriptions")]
        public IList<PubSubSubscription> Subscriptions { get; set; }

        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; set; }
    }
}