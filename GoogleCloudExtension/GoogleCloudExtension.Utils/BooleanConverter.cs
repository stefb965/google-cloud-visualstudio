// Copyright 2015 Google Inc. All Rights Reserved.
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
    /// General boolean converter, converting True or False to the desired values.
    /// </summary>
    public class BooleanConverter : MarkupExtension, IValueConverter
    {
        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? TrueValue : FalseValue;
            }

            Debug.WriteLine($"Value should be boolean: {value}");
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return (bool)value ? TrueValue : FalseValue;
            }

            Debug.WriteLine($"Value should be boolean: {value}");
            return DependencyProperty.UnsetValue;
        }

        //MarkupExtension implementation allows to use converter
        //directly in markup without creating static resource
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
