// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.ComponentModel.DataAnnotations;
using Google;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.Utils;
using Microsoft.VisualStudio.PlatformUI;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public class CreateTopicViewModel : ViewModelBase
    {
        private const string TopicNameRegex = "^(?!(?i)goog(?-i))[a-zA-Z]+[a-zA-Z0-9\\.\\-_~%+]*$";
        private const string TopicNameRegexErrorMessage = "Name must start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";
        private const string TopicNameLengthErrorMessage = "Name must be between 3 and 255 characters";
        private const string TopicHint = "Must be 3-255 characters, start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";

        private bool _validateOnChange;
        private string _topicName = string.Empty;
        private readonly ICloudExplorerSource _owner;
        private readonly DataSourceManager _dataManager;
        private readonly DialogWindow _window;

        public string TopicHintText => TopicHint;

        public string TopicNamePrefix => $"projects/{_owner?.CurrentProject?.ProjectId}/topics/";

        public WeakCommand CreateTopicCommand { get; }

        [MinLength(3, ErrorMessage = TopicNameLengthErrorMessage)]
        [MaxLength(255, ErrorMessage = TopicNameLengthErrorMessage)]
        [RegularExpression(TopicNameRegex, ErrorMessage = TopicNameRegexErrorMessage)]
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
                    ValidatePropertyAsync(_topicName);
                }
            }
        }

        public CreateTopicViewModel(ICloudExplorerSource owner, DialogWindow window)
        {
            _owner = owner;
            _window = window;

            _dataManager = new DataSourceManager(owner);

            CreateTopicCommand = new WeakCommand(OnCreateTopic);
        }

        protected override void OnValidationFinished(bool hasErrors)
        {
            base.OnValidationFinished(hasErrors);

            _window.Dispatcher.Invoke(() =>
            {
                CreateTopicCommand.CanExecuteCommand = !hasErrors;
            });
        }

        private async void OnCreateTopic()
        {
            _validateOnChange = true;
            await ValidateAsync();
            if (HasErrors) return;

            IsLoading = true;
            var topicFullName = TopicNamePrefix + TopicName;

            GcpOutputWindow.Activate();

            try
            {
                GcpOutputWindow.OutputLine($"Creating topic \"{topicFullName}\"");
                await _dataManager.PubSub.CreateTopicAsync(topicFullName);
                GcpOutputWindow.OutputLine($"Topic \"{topicFullName}\" has been created");

                _window.Close();
                _owner.Refresh();
            }
            catch (GoogleApiException ex)
            {
                GcpOutputWindow.OutputLine(ex.Message);
                UserPromptUtils.ErrorPrompt(ex.Message, "Error");
            }
            catch (Exception ex)
            {
                GcpOutputWindow.OutputLine(ex.Message);
                UserPromptUtils.ErrorPrompt(ex.Message, "Error");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
