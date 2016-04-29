// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using System.Windows.Threading;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.CloudExplorerSources.PubSub.Common;
using GoogleCloudExtension.Utils;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public class CreateTopicViewModel : DataViewModelBase
    {
        private readonly Dispatcher _dispatcher;
        private static readonly Lazy<ImageSource> s_hintIcon = new Lazy<ImageSource>(() => ResourceUtils.LoadResource(IconResourcePath));

        private const string TopicNameRegex = "^(?!(?i)goog(?-i))[a-zA-Z]+[a-zA-Z0-9\\.\\-_~%+]*$";
        private const string TopicHint = "Must be 3-255 characters, start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";
        private const string IconResourcePath = "CloudExplorerSources/PubSub/Resources/hint.png";

        private bool _validateOnChange;
        private string _topicName;

        public ImageSource HintIcon => s_hintIcon.Value;

        public string TopicHintText => TopicHint;

        public string TopicNamePrefix => $"projects/{Owner?.CurrentProject?.ProjectId}/topics/";

        public WeakCommand CreateTopicCommand { get; private set; }

        [MinLength(3)]
        [MaxLength(255)]
        [RegularExpression(TopicNameRegex)]
        public string TopicName
        {
            get
            {
                return _topicName;
            }
            set
            {
                SetValueAndRaise(ref _topicName, value);

                if (_validateOnChange)
                {
                    ValidateAsync();
                }
            }
        }

        public CreateTopicViewModel(ICloudExplorerSource owner,
            Dispatcher dispatcher)
            : base(owner)
        {
            _dispatcher = dispatcher;
            CreateTopicCommand = new WeakCommand(OnCreateTopic);
        }

        protected override void ValidationFinished(bool hasErrors)
        {
            base.ValidationFinished(hasErrors);

            _dispatcher.Invoke(() =>
            {
                CreateTopicCommand.CanExecuteCommand = !hasErrors;
            });
        }

        private async void OnCreateTopic()
        {
            _validateOnChange = true;
            await ValidateAsync();
            if (HasErrors) return;
        }
    }
}
