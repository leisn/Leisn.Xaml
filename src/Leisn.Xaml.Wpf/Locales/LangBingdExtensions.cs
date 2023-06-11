using Leisn.Common.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Leisn.Xaml.Wpf.Locales
{
    public static class LangBingdExtensions
    {
        public static void SetBindingLangKey(this DependencyObject targetObject, DependencyProperty targetProperty, string key)
        {
            Binding binding = new()
            {
                Source = Lang.Current,
                Path = new PropertyPath($"Values[{key}]"),
                Mode = BindingMode.OneWay,
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
