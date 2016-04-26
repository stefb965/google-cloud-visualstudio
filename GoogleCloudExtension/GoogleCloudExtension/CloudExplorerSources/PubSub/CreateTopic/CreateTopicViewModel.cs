// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Windows.Media;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.CreateTopic
{
    internal class CreateTopicViewModel
    {
        private const string TopicHint = "Must be 3-255 characters, start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/hint.png";
        private static readonly Lazy<ImageSource> s_pubSubIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        public ImageSource RootIcon => s_pubSubIcon.Value;

        public string TopichintText => TopicHint;
    }
}
