// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Google;
using Google.Apis.Pubsub.v1.Data;
using GoogleCloudExtension.CloudExplorer;
using GoogleCloudExtension.Utils;
using Microsoft.VisualStudio.PlatformUI;

namespace GoogleCloudExtension.CloudExplorerSources.PubSub.Windows
{
    public class CreateEditSubscriptionViewModel : ViewModelBase
    {
        private const string SubscriptionNameRegex = "^(?!(?i)goog(?-i))[a-zA-Z]+[a-zA-Z0-9\\.\\-_~%+]*$";
        private const string SubscriptionNameHint = "Must be 3-255 characters, start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";
        private const string SubscriptionNameRegexErrorMessage = "Name must start with an alphanumeric character, and contain only the following characters: letters, numbers, dashes (-), periods (.), underscores (_), tildes (~), percents (%) or plus signs (+). Cannot start with goog.";
        private const string SubscriptionNameLengthErrorMessage = "Name must be between 3 and 255 characters";
        private const string DeliveryTypeHint = "If Push, Pub/Sub delivers messages as soon as they are published. If Pull, subscribers must request delivery.";
        private const string PushEndpointUrlHint = "The URL of the service that receives push messages";
        private const string AcknowledgmentDeadlineHint = "How long Pub/Sub waits for the subscriber to acknowledge receipt before resending the message";
        private const string AcknowledgmentDeadlineValueErrorMessage = "Acknowledgment Deadline must be between 0 and 600 seconds";

        private readonly ICloudExplorerSource _owner;
        private readonly DataSourceManager _dataManager;
        private readonly DialogWindow _window;
        private readonly Subscription _subscription;

        private bool _validateOnChange;
        private string _dialogTitle;
        private string _okButtonText;
        private string _subscriptionName = string.Empty;
        private bool _isPull = true;
        private string _pushEndpointUrl = string.Empty;
        private int _ackDeadlineSeconds = 10;

        public string SubscriptionHintText => SubscriptionNameHint;
        public string DeliveryTypeHintText => DeliveryTypeHint;
        public string PushEndpointUrlHintText => PushEndpointUrlHint;
        public string AcknowledgmentDeadlineHintText => AcknowledgmentDeadlineHint;
        public string SubscriptionNamePrefix => $"projects/{_owner?.CurrentProject?.ProjectId}/subscriptions/";
        public Topic Topic { get; }

        public WeakCommand CreateTopicCommand { get; }

        public bool IsCreateMode => _subscription == null;
        public bool IsEditMode => _subscription != null;

        public string DialogTitle
        {
            get
            {
                return _dialogTitle;
            }
            set
            {
                SetValueAndRaise(ref _dialogTitle, value);
            }
        }

        public string OkButtonText
        {
            get
            {
                return _okButtonText;
            }
            set
            {
                SetValueAndRaise(ref _okButtonText, value);
            }
        }

        [MinLength(3, ErrorMessage = SubscriptionNameLengthErrorMessage)]
        [MaxLength(255, ErrorMessage = SubscriptionNameLengthErrorMessage)]
        [RegularExpression(SubscriptionNameRegex, ErrorMessage = SubscriptionNameRegexErrorMessage)]
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

        [Range(0, 600, ErrorMessage = AcknowledgmentDeadlineValueErrorMessage)]
        public int AckDeadlineSeconds
        {
            get
            {
                return _ackDeadlineSeconds;
            }
            set
            {
                SetValueAndRaise(ref _ackDeadlineSeconds, value);

                if (_validateOnChange)
                {
                    ValidatePropertyAsync(_ackDeadlineSeconds);
                }
            }
        }

        public CreateEditSubscriptionViewModel(ICloudExplorerSource owner, DialogWindow window,
            Topic topic, Subscription subscription)
        {
            _owner = owner;
            _window = window;
            _subscription = subscription;
            Topic = topic;

            InitSubscription();

            _dataManager = new DataSourceManager(owner);
            CreateTopicCommand = new WeakCommand(OnCreateTopic);
        }

        private void InitSubscription()
        {
            DialogTitle = _subscription == null
                ? "Create a new subscription"
                : "Edit subscription";

            OkButtonText = _subscription == null
                ? "Create"
                : "Save";

            if (_subscription != null)
            {
                SubscriptionName = _subscription.Name.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                IsPull = string.IsNullOrWhiteSpace(_subscription?.PushConfig?.PushEndpoint);
                PushEndpointUrl = _subscription?.PushConfig?.PushEndpoint;
                AckDeadlineSeconds = _subscription.AckDeadlineSeconds ?? 0;
            }
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
            var subscriptionFullName = SubscriptionNamePrefix + SubscriptionName;

            GcpOutputWindow.Activate();

            try
            {
                if (_subscription == null)
                {
                    GcpOutputWindow.OutputLine($"Creating subscription \"{subscriptionFullName}\"");
                    await _dataManager.PubSub.CreateSubscriptionAsync(Topic.Name, subscriptionFullName,
                        !IsPull ? PushEndpointUrl : null, AckDeadlineSeconds);
                    GcpOutputWindow.OutputLine($"Subscription \"{subscriptionFullName}\" has been created");

                    _window.Close();
                    _owner.Refresh();
                }
                else
                {

                    var hasOldPushConfig = !string.IsNullOrWhiteSpace(_subscription?.PushConfig?.PushEndpoint);
                    var hasPushConfig = !IsPull && !string.IsNullOrWhiteSpace(PushEndpointUrl);

                    if (hasOldPushConfig != hasPushConfig ||
                        (hasOldPushConfig && _subscription?.PushConfig?.PushEndpoint != PushEndpointUrl))
                    {
                        GcpOutputWindow.OutputLine($"Modifying push config for subscription \"{subscriptionFullName}\"");
                        await _dataManager.PubSub.ModifyPushConfig(subscriptionFullName, !IsPull ? PushEndpointUrl : null);
                        GcpOutputWindow.OutputLine("Push config been modified");
                        _owner.Refresh();
                    }

                    _window.Close();
                }
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
