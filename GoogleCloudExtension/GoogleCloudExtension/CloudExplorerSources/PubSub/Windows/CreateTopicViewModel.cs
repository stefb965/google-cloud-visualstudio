// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Windows.Media;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    internal class CreateTopicViewModel : ViewModelBase
    {
        private static readonly Lazy<ImageSource> s_hintIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        private const string TopicHint = "Must be 3-255 characters, start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/hint.png";

        private readonly ICloudExplorerSource _owner;
        private readonly DataSourceManager _dataManager;

        public ImageSource HintIcon => s_hintIcon.Value;

        public string TopicHintText => TopicHint;

        public string TopicNamePrefix => $"projects/{_owner?.CurrentProject?.ProjectId}/topics/";

        public CreateTopicViewModel(ICloudExplorerSource owner)
        {
            _owner = owner;
            _dataManager = new DataSourceManager(owner);
        }
    }
}
