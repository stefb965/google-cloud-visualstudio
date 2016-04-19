// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GoogleCloudExtension.Accounts;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.DataSources;
using GoogleCloudExtension.DataSources.Models;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    internal class PubSubSourceRootViewModel : SourceRootViewModelBase
    {
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/pubsub.png";
        private static readonly Lazy<ImageSource> s_pubSubIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        private static readonly TreeLeaf s_loadingPlaceholder = new TreeLeaf
        {
            Content = "Loading topics...",
            IsLoading = true
        };
        private static readonly TreeLeaf s_noItemsPlacehoder = new TreeLeaf
        {
            Content = "No topics found."
        };
        private static readonly TreeLeaf s_errorPlaceholder = new TreeLeaf
        {
            Content = "Failed to list topics.",
            IsError = true
        };

        private IList<PubSubTopic> _topics;
        private IList<PubSubSubscription> _subscriptions;

        public override ImageSource RootIcon => s_pubSubIcon.Value;

        public override string RootCaption => "Google Cloud Pub/Sub";

        public override TreeLeaf ErrorPlaceholder => s_errorPlaceholder;

        public override TreeLeaf LoadingPlaceholder => s_loadingPlaceholder;

        public override TreeLeaf NoItemsPlaceholder => s_noItemsPlacehoder;

        public override void Initialize(ICloudExplorerSource owner)
        {
            base.Initialize(owner);

            var menuItems = new List<FrameworkElement>
            {
                new MenuItem { Header = "Create topic", Command = new WeakCommand(OnCreateTopic) },
                new MenuItem { Header = "Browse topics", Command = new WeakCommand(OnBrowseTopics) },
            };

            ContextMenu = new ContextMenu { ItemsSource = menuItems };
        }

        private void OnCreateTopic()
        {
            MessageBox.Show("Not implemented yet", "Error", MessageBoxButton.OK);
        }

        private void OnBrowseTopics()
        {
            var url = $"https://console.cloud.google.com/cloudpubsub/topicList?project={Owner.CurrentProject.Id}";
            Debug.WriteLine($"Starting topics browsing at: {url}");
            Process.Start(url);
        }

        protected override async Task LoadDataOverride()
        {
            try
            {
                _topics = await LoadTopicList();
                _subscriptions = await LoadSubscriptionList();

                PresentTopicViewModels();
            }
            catch (DataSourceException ex)
            {
                GcpOutputWindow.OutputLine("Failed to load the list of Pub/Sub topics.");
                GcpOutputWindow.OutputLine(ex.Message);
                GcpOutputWindow.Activate();

                throw new CloudExplorerSourceException(ex.Message, ex);
            }
        }

        private void PresentTopicViewModels()
        {
            Children.Clear();
            if (_topics == null)
            {
                Children.Add(s_errorPlaceholder);
                return;
            }

            var topics = _topics.Select(x => new TopicViewModel(this, x, _subscriptions.Where(c => c.Topic == x.Name))).ToList();

            foreach (var item in topics)
            {
                Children.Add(item);
            }
            if (Children.Count == 0)
            {
                Children.Add(s_noItemsPlacehoder);
            }
        }

        private async Task<IList<PubSubTopic>> LoadTopicList()
        {
            var oauthToken = await AccountsManager.GetAccessTokenAsync();
            return await PubSubDataSource.GetTopicListAsync(Owner.CurrentProject.Id, oauthToken);
        }

        private async Task<IList<PubSubSubscription>> LoadSubscriptionList()
        {
            var oauthToken = await AccountsManager.GetAccessTokenAsync();
            return await PubSubDataSource.GetSubscriptionListAsync(Owner.CurrentProject.Id, oauthToken);
        }
    }
}