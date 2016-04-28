// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using GoogleCloudExtension.CloudExplorer;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public partial class CreateTopicDialog
    {
        private readonly CreateTopicViewModel _viewModel;

        public CreateTopicDialog(ICloudExplorerSource owner)
        {
            InitializeComponent();

            _viewModel = new CreateTopicViewModel(owner);
            DataContext = _viewModel;
        }
    }
}
