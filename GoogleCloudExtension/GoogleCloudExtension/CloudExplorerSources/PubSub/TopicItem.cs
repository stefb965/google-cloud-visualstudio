// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.ComponentModel;
using System.Linq;
using Google.Apis.Pubsub.v1.Data;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    internal class TopicItem
    {
        private const string Category = "Topic Properties";

        private readonly Topic _topic;

        public TopicItem(Topic topic)
        {
            _topic = topic;
            Name = _topic.Name.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }

        [Category(Category)]
        [Description("Full name of the topic")]
        public string FullName => _topic.Name;

        [Category(Category)]
        [Description("The name of the topic")]
        public string Name { get; }
    }
}