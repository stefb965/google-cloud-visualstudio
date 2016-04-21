// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System.Windows;
using Microsoft.VisualStudio.PlatformUI;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Dialogs
{
    public class CreateEditTopicDialog : DialogWindow
    {
        public CreateEditTopicDialog()
        {
            Title = "Topic";
            Width = 500;
            Height = 500;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
    }
}
