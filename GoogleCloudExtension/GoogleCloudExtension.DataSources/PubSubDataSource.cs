// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Pubsub.v1;
using Google.Apis.Pubsub.v1.Data;

namespace GoogleCloudExtension.DataSources
{
    /// <summary>
    /// Data source that returns information about Pub/Sub topics and subscriptions for a particular project and credentials.
    /// </summary>
    public class PubSubDataSource : DataSourceBase<PubsubService>
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="credential"></param>
        public PubSubDataSource(string projectId, GoogleCredential credential) : base(projectId, () => CreateService(credential))
        { }

        private static PubsubService CreateService(GoogleCredential credential)
        {
            return new PubsubService(new Google.Apis.Services.BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
            });
        }

        /// <summary>
        /// Fetches the list of topics for the given project.
        /// </summary>
        /// <returns>The list of topics.</returns>
        public Task<IList<Topic>> GetTopicListAsync()
        {
            return LoadPagedListAsync(token =>
            {
                if (String.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Loading final page.");
                    return Service.Projects.Topics.List($"projects/{ProjectId}").ExecuteAsync();
                }
                else
                {
                    Debug.WriteLine($"Loading page: {token}");
                    var request = Service.Projects.Topics.List($"projects/{ProjectId}");
                    request.PageToken = token;
                    return request.ExecuteAsync();
                }
            },
                x => x.Topics,
                x => x.NextPageToken);
        }

        /// <summary>
        /// Fetches the list of subscriptions for the given project.
        /// </summary>
        /// <returns>The list of subscriptions.</returns>
        public Task<IList<Subscription>> GetSubscriptionListAsync()
        {
            return LoadPagedListAsync(token =>
            {
                if (String.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("Loading final page.");
                    return Service.Projects.Subscriptions.List($"projects/{ProjectId}").ExecuteAsync();
                }
                else
                {
                    Debug.WriteLine($"Loading page: {token}");
                    var request = Service.Projects.Subscriptions.List($"projects/{ProjectId}");
                    request.PageToken = token;
                    return request.ExecuteAsync();
                }
            },
                x => x.Subscriptions,
                x => x.NextPageToken);
        }

        /// <summary>
        /// Creates new topic.
        /// </summary>
        /// <param name="topicId">The id of the topic to be created.</param>
        public async Task CreateTopicAsync(string topicId)
        {
            var topic = new Topic();
            await Service.Projects.Topics.Create(topic, topicId).ExecuteAsync();
        }

        /// <summary>
        /// Creates new subscription
        /// </summary>
        /// <param name="topicName">The name of the topic with which new subscription will be associated.</param>
        /// <param name="subscriptionName">The name of the subscription to be created.</param>
        /// <param name="pushEndpointUrl">Url of push endpoint.</param>
        /// <param name="ackDeadlineSeconds">Acknowledgment deadline in seconds.</param>
        /// <returns></returns>
        public async Task CreateSubscriptionAsync(string topicName, string subscriptionName,
            string pushEndpointUrl, int ackDeadlineSeconds)
        {
            var subscription = new Subscription
            {
                Topic = topicName,
                AckDeadlineSeconds = ackDeadlineSeconds
            };

            if (!string.IsNullOrWhiteSpace(pushEndpointUrl))
            {
                subscription.PushConfig = new PushConfig
                {
                    PushEndpoint = pushEndpointUrl
                };
            }

            await Service.Projects.Subscriptions.Create(subscription, subscriptionName).ExecuteAsync();
        }

        /// <summary>
        /// Deletes given topic.
        /// </summary>
        /// <param name="topicId">The id of the topic to be deleted.</param>
        public async Task DeleteTopicAsync(string topicId)
        {
            await Service.Projects.Topics.Delete(topicId).ExecuteAsync();
        }

        /// <summary>
        /// Deletes given subscription.
        /// </summary>
        /// <param name="subscriptionId">The id of the subscription to be deleted.</param>
        public async Task DeleteSubscriptionAsync(string subscriptionId)
        {
            await Service.Projects.Subscriptions.Delete(subscriptionId).ExecuteAsync();
        }
    }
}
