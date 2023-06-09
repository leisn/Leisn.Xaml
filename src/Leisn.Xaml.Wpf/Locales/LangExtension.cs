using Leisn.Common.Helpers;

using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Leisn.Xaml.Wpf.Locales
{
    public class LangExtension : MarkupExtension
    {
        public string Key { get; }
        public BindingBase? Binding { get; }

        public LangExtension(string key)
        {
            Key = key;
        }

        public LangExtension(string key, BindingBase binding) : this(key)
        {
            Binding = binding;
            Lang.LangChanged += I18N_LangChanged;
        }

        private void I18N_LangChanged()
        {
            _bindingExpression?.UpdateTarget();
        }

        private BindingExpressionBase? _bindingExpression;
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //if (serviceProvider.GetService(typeof(IProvideValueTarget)) is not IProvideValueTarget provideValueTarget)
            //    return this;
            //if (provideValueTarget.TargetObject is not DependencyObject targetObject)
            //    return this;
            //if (provideValueTarget.TargetProperty is not DependencyProperty targetProperty)
            //    return this;
            if (Binding == null)
            {
                Binding binding = new()
                {
                    Source = Lang.Current,
                    Path = new PropertyPath($"Values[{Key}]"),
                    Mode = BindingMode.OneWay
                };
                //BindingOperations.SetBinding(targetObject, targetProperty, binding);
                return binding.ProvideValue(serviceProvider);
            }
            if (Binding is Binding b)
            {
                LangValueConverter converter = new(b.Converter, b.ConverterParameter);
                b.Converter = converter;
                b.ConverterParameter = Key;
            }
            else if (Binding is MultiBinding mb)
            {
                LangMultiValueConverter converter = new(mb.Converter, mb.ConverterParameter);
                mb.Converter = converter;
                mb.ConverterParameter = Key;
            }

            _bindingExpression = (BindingExpressionBase)Binding.ProvideValue(serviceProvider);
            return _bindingExpression;
        }

        public static void SetBinding(DependencyObject targetObject, DependencyProperty targetProperty, string key)
        {
            Binding binding = new()
            {
                Source = Lang.Current,
                Path = new PropertyPath($"Values[{key}]"),
                Mode = BindingMode.OneWay,
            };
            _ = BindingOperations.SetBinding(targetObject, targetProperty, binding);
        }

        public static void SetBindingFormat(DependencyObject targetObject, DependencyProperty targetProperty, string format)
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
