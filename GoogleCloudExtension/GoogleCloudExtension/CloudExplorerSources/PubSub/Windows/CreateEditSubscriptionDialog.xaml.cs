// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System.Windows;
using Google.Apis.Pubsub.v1.Data;
using GoogleCloudExtension.CloudExplorer;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public partial class CreateEditSubscriptionDialog
    {
        private readonly CreateEditSubscriptionViewModel _viewModel;
        private Style _style;

        public CreateEditSubscriptionDialog(ICloudExplorerSource owner, Topic topic)
        {
            InitializeComponent();

            _viewModel = new CreateEditSubscriptionViewModel(owner, this, topic);
            DataContext = _viewModel;

            //_style = (Style)FindResource(VsResourceKeys.CheckBoxStyleKey);

        }
    }
}
