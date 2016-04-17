// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.DataSources.Models;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    internal class SubscriptionViewModel : TreeHierarchy, ICloudExplorerItemSource
    {
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/item_icon.png";
        private static readonly Lazy<ImageSource> s_subscriptionIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        private readonly PubSubSourceRootViewModel _owner;
        private readonly Lazy<SubscriptionItem> _item;

        public object Item => _item.Value;
        public event EventHandler ItemChanged;

        public SubscriptionViewModel(PubSubSourceRootViewModel owner, PubSubSubscription subscription)
        {
            _owner = owner;
            _item = new Lazy<SubscriptionItem>(() => new SubscriptionItem(subscription));

            Content = _item.Value.Name;
            Icon = s_subscriptionIcon.Value;

            var menuItems = new List<FrameworkElement>
            {
                new MenuItem { Header = "Edit subscription", Command = new WeakCommand(OnEditSubscription) },
                new MenuItem { Header = "Browse subscription", Command = new WeakCommand(OnBrowseSubscription) },
                new Separator(),
                new MenuItem { Header = "Delete subscription", Command = new WeakCommand(OnDeleteSubscription) },
            };

            ContextMenu = new ContextMenu { ItemsSource = menuItems };
        }

        private void OnEditSubscription()
        {
            MessageBox.Show("Not implemented yet", "Error", MessageBoxButton.OK);
        }

        private void OnBrowseSubscription()
        {
            var url = $"https://pantheon.corp.google.com/cloudpubsub/subscriptions/{_item.Value.Name}?edit=false&project={_owner.Owner.CurrentProject.Id}";
            Debug.WriteLine($"Starting subscription browsing at: {url}");
            Process.Start(url);
        }

        private void OnDeleteSubscription()
        {
            MessageBox.Show("Not implemented yet", "Error", MessageBoxButton.OK);
        }
    }
}