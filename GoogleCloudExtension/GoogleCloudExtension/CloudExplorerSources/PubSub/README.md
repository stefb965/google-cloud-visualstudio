# Pub/Sub Cloud Explorer Source

This folder contains the source code for the Pub/Sub Cloud Explorer Source.

Pub/Sub Cloud Explorer Source supports listing, creation and deletion of topics and subscriptions, publishing to topic and pulling from subscription. 

More info about [Google Cloud Pub/Sub](https://cloud.google.com/pubsub/overview)

This cloud explorer source has 2 dialogs and 2 tool windows. Dialogs are shown as a separate windows and they are inherited from DialogWindow base class. And tool windows that are inherited from ToolWindowPane base class are shown inside main Visual Studio window (but they can be detached) side by side with the source code.

Dialogs:
* CreateTopicDialog
* CreateEditSubscriptionDialog

Tool windows:
* PublishToolWindow
* PullToolWindow

Non-standard controls was used:
* PrefixedTextBox
* NumericTextBox
* HintedLabel

Default templates for the controls are specified in /Themes/Generic.xaml resource dictionary. 

All UI elements support Visual Studio themes. Styles for this support are defined in /Controls/Styles.xaml

View models for all windows are inherited from ViewModelBase base class that implement INotifyDataErrorInfo, INotifyPropertyChanged and has properties for loading state. This allows to create fully-featured UI with data validation and loading indication. 