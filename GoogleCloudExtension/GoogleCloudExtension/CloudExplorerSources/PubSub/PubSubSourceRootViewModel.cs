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
using Google.Apis.Pubsub.v1.Data;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.CloudExplorerSources.PubSub.Windows;
using GoogleCloudExtension.DataSources;
using GoogleCloudExtension.Utils;
using GoogleCloudExtension.Accounts;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    public class PubSubSourceRootViewModel : SourceRootViewModelBase
    {
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/pubsub.png";
        private static readonly Lazy<ImageSource> s_pubSubIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadImage(IconResourcePath));

        private static readonly TreeLeaf s_loadingPlaceholder = new TreeLeaf
        {
            Caption = "Loading topics...",
            IsLoading = true
        };
        private static readonly TreeLeaf s_noItemsPlacehoder = new TreeLeaf
        {
            Caption = "No topics found."
        };
        private static readonly TreeLeaf s_errorPlaceholder = new TreeLeaf
        {
            Caption = "Failed to list topics.",
            IsError = true
        };

        private IList<Topic> _topics;
        private IList<Subscription> _subscriptions;

        public DataSourceManager DataManager { get; private set; }

        public override ImageSource RootIcon => s_pubSubIcon.Value;

        public override string RootCaption => "Google Cloud Pub/Sub";

        public override TreeLeaf ErrorPlaceholder => s_errorPlaceholder;

        public override TreeLeaf LoadingPlaceholder => s_loadingPlaceholder;

        public override TreeLeaf NoItemsPlaceholder => s_noItemsPlacehoder;

        public override void Initialize()
        {
            base.Initialize();

            InvalidateProjectOrAccount(); 

            var menuItems = new List<FrameworkElement>
            {
                new MenuItem { Header = "Create topic", Command = new WeakCommand(OnCreateTopic) },
                new MenuItem { Header = "Browse topics", Command = new WeakCommand(OnBrowseTopics) },
            };

            ContextMenu = new ContextMenu { ItemsSource = menuItems };
        }

        public override void InvalidateProjectOrAccount()
        {
            Debug.WriteLine("New credentials, invalidating the PubSub source.");
            DataManager = new DataSourceManager();
        }

        private void OnCreateTopic()
        {
            var dlg = new CreateTopicDialog(this);
            dlg.ShowModal();
        }

        private void OnBrowseTopics()
        {
            var url = $"https://console.cloud.google.com/cloudpubsub/topicList?project={CredentialsStore.Default?.CurrentProjectId}";
            Debug.WriteLine($"Starting topics browsing at: {url}");
            Process.Start(url);
        }

        protected override async Task LoadDataOverride()
        {
            try
            {
                _topics = await DataManager.PubSub.GetTopicListAsync();
                _subscriptions = await DataManager.PubSub.GetSubscriptionListAsync();

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
    }
}