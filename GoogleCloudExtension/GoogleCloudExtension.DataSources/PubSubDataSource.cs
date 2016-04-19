// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using GoogleCloudExtension.DataSources.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            var url = $"https://pubsub.googleapis.com/v1/projects/{projectId}/topics";
            var client = new WebClient().SetOauthToken(oauthToken);

            return ApiHelpers.LoadPagedListAsync<PubSubTopic, PubSubTopicPage>(
                client,
                url,
                x => x.Topics,
                x => string.IsNullOrEmpty(x.NextPageToken) ? null : $"{url}&pageToken={x.NextPageToken}");
        }

        /// <summary>
        /// Fetches the list of subscriptions for the given project.
        /// </summary>
        /// <param name="projectId">The id of the project that owns the subscriptions.</param>
        /// <param name="oauthToken">The oauth token to use to authorize the call.</param>
        /// <returns>The list of subscriptions.</returns>
        public static Task<IList<PubSubSubscription>> GetSubscriptionListAsync(string projectId, string oauthToken)
        {
            var url = $"https://pubsub.googleapis.com/v1/projects/{projectId}/subscriptions";
            var client = new WebClient().SetOauthToken(oauthToken);

            return ApiHelpers.LoadPagedListAsync<PubSubSubscription, PubSubSubscriptionPage>(
                client,
                url,
                x => x.Subscriptions,
                x => string.IsNullOrEmpty(x.NextPageToken) ? null : $"{url}&pageToken={x.NextPageToken}");
        }

        /// <summary>
        /// Deletes given topic.
        /// </summary>
        /// <param name="topicId">The id of the topic to be deleted.</param>
        /// <param name="oauthToken">The oauth token to use to authorize the call.</param>
        public static async Task DeleteTopicAsync(string topicId, string oauthToken)
        {
            var url = $"https://pubsub.googleapis.com/v1/{topicId}";
            var client = new ExtendedWebClient();
            client.SetOauthToken(oauthToken);
            client.HttpMethod = "DELETE";

            try
            {
                var response = await client.DownloadStringTaskAsync(url);
            }
            catch (WebException ex)
            {
                Debug.WriteLine($"Request failed: {ex.Message}");
                throw new DataSourceException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Deletes given subscription.
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to be deleted.</param>
        /// <param name="oauthToken">The oauth token to use to authorize the call.</param>
        public static async Task DeleteSubscriptionAsync(string subscriptionId, string oauthToken)
        {
            var url = $"https://pubsub.googleapis.com/v1/{subscriptionId}";
            var client = new ExtendedWebClient();
            client.SetOauthToken(oauthToken);
            client.HttpMethod = "DELETE";
               
            try
            {
                var response = await client.DownloadStringTaskAsync(url);
            }
            catch (WebException ex)
            {
                Debug.WriteLine($"Request failed: {ex.Message}");
                throw new DataSourceException(ex.Message, ex);
            }
        }
    }
}
