// By Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Locales
{
    internal class LangValueConverter : IValueConverter
    {
        private readonly IValueConverter? _converter;
        private readonly object? _parameter;
        public LangValueConverter(IValueConverter? converter, object? parameter)
        {
            _converter = converter;
            _parameter = parameter;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object v = _converter != null ? _converter.Convert(value, targetType, _parameter, culture) : value;
            return string.Format(Lang.Get((string)parameter), v);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    internal class LangMultiValueConverter : IMultiValueConverter
    {
        private readonly IMultiValueConverter? _converter;
        private readonly object? _parameter;
        public LangMultiValueConverter(IMultiValueConverter? converter, object? parameter)
        {
            _converter = converter;
            _parameter = parameter;
        }
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object v = _converter != null ? _converter.Convert(values, targetType, _parameter, culture) : values;
            return string.Format(Lang.Get((string)parameter), v);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
