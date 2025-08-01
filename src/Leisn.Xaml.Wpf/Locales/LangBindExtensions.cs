﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Data;

using Leisn.Common.Helpers;

namespace Leisn.Xaml.Wpf.Locales
{
    public static class LangBindExtensions
    {
        public static void SetBindingLangKey(this DependencyObject targetObject, DependencyProperty targetProperty, string key, string? stringFormat = null)
        {
            Binding binding = new()
            {
                Source = Lang.Current,
                Path = new PropertyPath($"Values[{key}]"),
                Mode = BindingMode.OneWay,
                StringFormat = stringFormat
            };
            _ = BindingOperations.SetBinding(targetObject, targetProperty, binding);
        }

        public static void SetBindingLangFormat(this DependencyObject targetObject, DependencyProperty targetProperty, string format)
        {
            string[] keys = StringHelper.ParseFormat(format, out string? convertedFormat);
            Binding binding = new()
            {
                Source = Lang.Current,
                Path = new PropertyPath($"Values"),
                Mode = BindingMode.OneWay,
                ConverterParameter = new LangFormat { Format = convertedFormat, Keys = keys },
                Converter = LangFormatConverter.Instance
            };
            _ = BindingOperations.SetBinding(targetObject, targetProperty, binding);
        }
    }
}
