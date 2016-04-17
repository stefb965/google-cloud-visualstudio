// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.DataSources.Models;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    internal class TopicViewModel : TreeHierarchy, ICloudExplorerItemSource
    {
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/item_icon.png";
        private static readonly Lazy<ImageSource> s_topicIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        private readonly PubSubSourceRootViewModel _owner;
        private readonly Lazy<TopicItem> _item;

        public object Item => _item.Value;
        public event EventHandler ItemChanged;

        public TopicViewModel(PubSubSourceRootViewModel owner, PubSubTopic topic, IEnumerable<PubSubSubscription> subscriptions)
        {
            _owner = owner;
            _item = new Lazy<TopicItem>(() => new TopicItem(topic));

            Content = _item.Value.Name;
            Icon = s_topicIcon.Value;

            var viewModels = subscriptions.Select(x => new SubscriptionViewModel(owner, x));

            foreach (var viewModel in viewModels)
            {
                Children.Add(viewModel);
            }

            var menuItems = new List<FrameworkElement>
            {
                new MenuItem { Header = "New subscription", Command = new WeakCommand(OnNewSubscription) },
                new MenuItem { Header = "Publish", Command = new WeakCommand(OnPublish) },
                new Separator(),
                new MenuItem { Header = "Delete topic", Command = new WeakCommand(OnDeleteTopic) },
            };

            ContextMenu = new ContextMenu { ItemsSource = menuItems };
        }

        private void OnNewSubscription()
        {
            MessageBox.Show("Not implemented yet", "Error", MessageBoxButton.OK);
        }

        private void OnPublish()
        {
            MessageBox.Show("Not implemented yet", "Error", MessageBoxButton.OK);
        }

        private void OnDeleteTopic()
        {
            MessageBox.Show("Not implemented yet", "Error", MessageBoxButton.OK);
        }
    }
}