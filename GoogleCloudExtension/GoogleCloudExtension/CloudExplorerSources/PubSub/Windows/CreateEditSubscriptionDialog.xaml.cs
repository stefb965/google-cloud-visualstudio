// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Windows;
using GoogleCloudExtension.CloudExplorer;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public partial class CreateEditSubscriptionDialog
    {
        private readonly CreateEditSubscriptionViewModel _viewModel;
        private Style _style;

        public CreateEditSubscriptionDialog(ICloudExplorerSource owner)
        {
            InitializeComponent();

            _viewModel = new CreateEditSubscriptionViewModel(owner, this);
            DataContext = _viewModel;

            _style = (Style)FindResource(VsResourceKeys.TextBoxStyleKey);

        }
    }
}
