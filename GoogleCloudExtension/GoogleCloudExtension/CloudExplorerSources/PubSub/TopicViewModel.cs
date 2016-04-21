// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GoogleCloudExtension.Accounts;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.CloudExplorerSources.PubSub.Dialogs;
using GoogleCloudExtension.CloudExplorerSources.PubSub.ToolWindows;
using GoogleCloudExtension.DataSources;
using GoogleCloudExtension.DataSources.Models;
using GoogleCloudExtension.Utils;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    internal class TopicViewModel : TreeHierarchy, ICloudExplorerItemSource
    {
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/item_icon.png";
        private static readonly Lazy<ImageSource> s_topicIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        private readonly PubSubSourceRootViewModel _owner;
        private readonly Lazy<TopicItem> _item;
        private readonly List<SubscriptionViewModel> _subscriptions;

        public object Item => _item.Value;
        public TopicItem TopicItem => _item.Value;

        public event EventHandler ItemChanged;

        public TopicViewModel(PubSubSourceRootViewModel owner, PubSubTopic topic, IEnumerable<PubSubSubscription> subscriptions)
        {
            _owner = owner;
            _item = new Lazy<TopicItem>(() => new TopicItem(topic));

            Content = _item.Value.Name;
            Icon = s_topicIcon.Value;

            _subscriptions = subscriptions.Select(x => new SubscriptionViewModel(owner, x)).ToList();

            foreach (var viewModel in _subscriptions)
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
            var dlg = new CreateEditSubscriptionDialog();
            dlg.ShowModal();
        }

        private static int _pubWindowId = 0;
        private void OnPublish()
        {
            _pubWindowId++;
            var window = GoogleCloudExtensionPackage.Instance.FindToolWindow(typeof(PublishToolWindow), _pubWindowId, true);

            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private async void OnDeleteTopic()
        {
            if (!UserPromptUtils.YesNoPrompt($"Do you want to delete the topic \"{_item.Value.FullName}\"?",
                "Confirm")) return;

            var oauthToken = await AccountsManager.GetAccessTokenAsync();

            GcpOutputWindow.Activate();

            foreach (var subscription in _subscriptions)
            {
                GcpOutputWindow.OutputLine($"Deleting \"{subscription.SubscriptionItem.FullName}\" subscription.");
                await PubSubDataSource.DeleteSubscriptionAsync(subscription.SubscriptionItem.FullName, oauthToken);
            }

            GcpOutputWindow.OutputLine($"Deleting \"{_item.Value.FullName}\" topic.");
            await PubSubDataSource.DeleteTopicAsync(_item.Value.FullName, oauthToken);
            _owner.Owner.Refresh();
        }
    }
}