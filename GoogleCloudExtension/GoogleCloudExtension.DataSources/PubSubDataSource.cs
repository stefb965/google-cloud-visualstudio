// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using GoogleCloudExtension.DataSources.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GoogleCloudExtension.DataSources
{
    /// <summary>
    /// Data source that returns information about Pub/Sub topics and subscriptions. Calls the Pub/Sub API according 
    /// to https://cloud.google.com/pubsub/overview.
    /// </summary>
    public static class PubSubDataSource
    {
        /// <summary>
        /// Fetches the list of topics for the given project.
        /// </summary>
        /// <param name="projectId">The id of the project that owns the topics.</param>
        /// <param name="oauthToken">The oauth token to use to authorize the call.</param>
        /// <returns>The list of topics.</returns>
        public static Task<IList<PubSubTopic>> GetTopicListAsync(string projectId, string oauthToken)
        {
            var baseUrl = $"https://pubsub.googleapis.com/v1/projects/{projectId}/topics";
            var client = new WebClient().SetOauthToken(oauthToken);

            return ApiHelpers.LoadPagedListAsync<PubSubTopic, PubSubTopicPage>(
                client,
                baseUrl,
                x => x.Topics,
                x => string.IsNullOrEmpty(x.NextPageToken) ? null : $"{baseUrl}&pageToken={x.NextPageToken}");
        }

        /// <summary>
        /// Fetches the list of subscriptions for the given project.
        /// </summary>
        /// <param name="projectId">The id of the project that owns the subscriptions.</param>
        /// <param name="oauthToken">The oauth token to use to authorize the call.</param>
        /// <returns>The list of subscriptions.</returns>
        public static Task<IList<PubSubSubscription>> GetSubscriptionListAsync(string projectId, string oauthToken)
        {
            var baseUrl = $"https://pubsub.googleapis.com/v1/projects/{projectId}/subscriptions";
            var client = new WebClient().SetOauthToken(oauthToken);

            return ApiHelpers.LoadPagedListAsync<PubSubSubscription, PubSubSubscriptionPage>(
                client,
                baseUrl,
                x => x.Subscriptions,
                x => string.IsNullOrEmpty(x.NextPageToken) ? null : $"{baseUrl}&pageToken={x.NextPageToken}");
        }
    }
}
