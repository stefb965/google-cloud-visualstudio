// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Windows.Media;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Dialogs
{
    internal class CreateEditTopicViewModel: Model
    {
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/hint.png";
        private static readonly Lazy<ImageSource> s_pubSubIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        public ImageSource RootIcon => s_pubSubIcon.Value;
    }
}
