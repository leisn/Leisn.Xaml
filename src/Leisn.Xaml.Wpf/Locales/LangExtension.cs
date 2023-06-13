// By Leisn (https://leisn.com , https://github.com/leisn)

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
            if (Binding == null)
            {
                Binding binding = new()
                {
                    Source = Lang.Current,
                    Path = new PropertyPath($"Values[{Key}]"),
                    Mode = BindingMode.OneWay
                };
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

    }

}
