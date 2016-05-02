// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.ComponentModel.DataAnnotations;
using Google;
using Google.Apis.Pubsub.v1.Data;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.CloudExplorerSources.PubSub.Common;
using GoogleCloudExtension.Utils;
using Microsoft.VisualStudio.PlatformUI;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public class CreateEditSubscriptionViewModel : DataViewModelBase
    {
        private const string SubscriptionNameRegex = "^(?!(?i)goog(?-i))[a-zA-Z]+[a-zA-Z0-9\\.\\-_~%+]*$";
        private const string SubscriptionNameHint = "Must be 3-255 characters, start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";
        private const string DeliveryTypeHint = "If Push, Pub/Sub delivers messages as soon as they are published. If Pull, subscribers must request delivery.";
        private const string PushEndpointUrlHint = "The URL of the service that receives push messages";
        private const string AcknowledgmentDeadlineHint = "How long Pub/Sub waits for the subscriber to acknowledge receipt before resending the message";

        private readonly ICloudExplorerSource _owner;
        private readonly DataSourceManager _dataManager;
        private readonly DialogWindow _window;
        private bool _validateOnChange;
        private string _subscriptionName = string.Empty;
        private bool _isPull = true;
        private string _pushEndpointUrl = string.Empty;
        private int _acknowledgmentDeadline = 10;

        public string SubscriptionHintText => SubscriptionNameHint;
        public string DeliveryTypeHintText => DeliveryTypeHint;
        public string PushEndpointUrlHintText => PushEndpointUrlHint;
        public string AcknowledgmentDeadlineHintText => AcknowledgmentDeadlineHint;
        public string SubscriptionNamePrefix => $"projects/{_owner?.CurrentProject?.ProjectId}/subscriptions/";
        public Topic Topic { get; }

        public WeakCommand CreateTopicCommand { get; }

        [MinLength(3)]
        [MaxLength(255)]
        [RegularExpression(SubscriptionNameRegex)]
        public string SubscriptionName
        {
            get
            {
                return _subscriptionName;
            }
            set
            {
                SetValueAndRaise(ref _subscriptionName, value);

                if (_validateOnChange)
                {
                    ValidatePropertyAsync(_subscriptionName);
                }
            }
        }

        [MaxLength(4096)]
        public string PushEndpointUrl
        {
            get
            {
                return _pushEndpointUrl;
            }
            set
            {
                SetValueAndRaise(ref _pushEndpointUrl, value);

                if (_validateOnChange)
                {
                    ValidatePropertyAsync(_pushEndpointUrl);
                }
            }
        }


        public bool IsPull
        {
            get
            {
                return _isPull;
            }
            set
            {
                SetValueAndRaise(ref _isPull, value);

                if (_validateOnChange)
                {
                    ValidatePropertyAsync(_isPull);
                }
            }
        }

        [Range(0, 600)]
        public int AcknowledgmentDeadline
        {
            get
            {
                return _acknowledgmentDeadline;
            }
            set
            {
                SetValueAndRaise(ref _acknowledgmentDeadline, value);

                if (_validateOnChange)
                {
                    ValidatePropertyAsync(_acknowledgmentDeadline);
                }
            }
        }

        public CreateEditSubscriptionViewModel(ICloudExplorerSource owner, DialogWindow window, Topic topic)
        {
            _owner = owner;
            _window = window;
            Topic = topic;

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

            Loading = true;
            var subscriptionFullName = SubscriptionNamePrefix + SubscriptionName;

            GcpOutputWindow.Activate();

            try
            {
                GcpOutputWindow.OutputLine($"Creating subscription \"{subscriptionFullName}\"");
                await _dataManager.PubSubDataSource.CreateSubscriptionAsync(Topic.Name, subscriptionFullName,
                    PushEndpointUrl, AcknowledgmentDeadline);
                GcpOutputWindow.OutputLine($"Subscription \"{subscriptionFullName}\" has been created");

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
                Loading = false;
            }
        }
    }
}
