// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.


namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public partial class CreateTopicDialog
    {
        private readonly CreateTopicViewModel _viewModel;

        public CreateTopicDialog(PubSubSourceRootViewModel root)
        {
            InitializeComponent();

            _viewModel = new CreateTopicViewModel(root, this);
            DataContext = _viewModel;
        }
    }
}
