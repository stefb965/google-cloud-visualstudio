// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.ComponentModel;
using System.Linq;
using GoogleCloudExtension.DataSources.Models;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    internal class SubscriptionItem
    {
        private const string Category = "Subscription Properties";

        private readonly PubSubSubscription _subscription;

        public SubscriptionItem(PubSubSubscription subscription)
        {
            _subscription = subscription;
            Name = subscription.Name.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }

        [Category(Category)]
        [Description("Full name of the subscription")]
        public string FullName => _subscription.Name;

        [Category(Category)]
        [Description("The name of the subscription")]
        public string Name { get; }

        [Category(Category)]
        [Description("The name of the topic")]
        public string Topic => _subscription.Topic;

        [Category(Category)]
        [Description("Delivery Type")]
        public string DeliveryType => !string.IsNullOrWhiteSpace(_subscription?.PushConfig?.PushEndpoint) ? "Push" : "Pull";

        [Category(Category)]
        [Description("Acknowledgment Deadline")]
        public string AckDeadlineSeconds => $"{_subscription.AckDeadlineSeconds} seconds";
    }
}