// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.VisualStudio.Shell.Interop;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub
{
    internal class SubscriptionViewModel : TreeHierarchy, ICloudExplorerItemSource
    {
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/item_icon.png";
        private static readonly Lazy<ImageSource> s_subscriptionIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        private readonly PubSubSourceRootViewModel _owner;
        private readonly Lazy<SubscriptionItem> _item;

        public object Item => _item.Value;
        public SubscriptionItem SubscriptionItem => _item.Value;

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
                new MenuItem { Header = "Pull", Command = new WeakCommand(OnPullSubscription) },
                new Separator(),
                new MenuItem { Header = "Delete subscription", Command = new WeakCommand(OnDeleteSubscription) },
            };

            ContextMenu = new ContextMenu { ItemsSource = menuItems };
        }

        private void OnEditSubscription()
        {
            var dlg = new CreateEditSubscriptionDialog();
            dlg.ShowModal();
        }

        private static int _pulWindowId = 0;
        public void OnPullSubscription()
        {
            _pulWindowId++;
            var window = GoogleCloudExtensionPackage.Instance.FindToolWindow(typeof(PullToolWindow), _pulWindowId, true);

            if (window?.Frame == null)
            {
                throw new NotSupportedException("Cannot create tool window");
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

        private void OnBrowseSubscription()
        {
            var url = $"https://console.cloud.google.com/cloudpubsub/subscriptions/{_item.Value.Name}?edit=false&project={_owner.Owner.CurrentProject.Id}";
            Debug.WriteLine($"Starting subscription browsing at: {url}");
            Process.Start(url);
        }

        private async void OnDeleteSubscription()
        {
            if (!UserPromptUtils.YesNoPrompt($"Do you want to delete the subscription \"{_item.Value.FullName}\"?",
                "Confirm")) return;

            GcpOutputWindow.Activate();
            GcpOutputWindow.OutputLine($"Deleting \"{_item.Value.FullName}\" subscription.");

            var oauthToken = await AccountsManager.GetAccessTokenAsync();
            await PubSubDataSource.DeleteSubscriptionAsync(_item.Value.FullName, oauthToken);
            _owner.Owner.Refresh();
        }
    }
}