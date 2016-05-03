// Copyright 2016 Google Inc. All Rights Reserved.
// Licensed under the Apache License Version 2.0.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace GoogleCloudExtension.Utils
{
    /// <summary>
    /// Converts a string value into a Visibility enum value. 
    /// Any non white space mean Visible and null or white space means Collapsed.
    /// Note: Only Convert is implemented, so this is not a bidirectional converter, do not use on TwoWay bindings.
    /// </summary>
    public class StringToVisibilityConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Whether logging the value being converted is enabled.
        /// </summary>
        public bool LoggingEnabled { get; set; }

        /// <summary>
        /// The prefix string to use for the log messages, useful for finding the entries in the 
        /// Output window.
        /// </summary>
        public string LoggingPrefix { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;

            var result = !string.IsNullOrWhiteSpace(val) ?
            Visibility.Visible : Visibility.Collapsed;

            if (LoggingEnabled)
            {
                Debug.WriteLine($"{nameof(StringToVisibilityConverter)}: {LoggingPrefix} converting {value} to {result}");
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inheriting from the MarkupExtension class allows us to use converter
        /// directly in markup without creating static resource for it.
        /// ProvideValue returns this because our markup extension
        /// is either implements IValueConverter interface.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
