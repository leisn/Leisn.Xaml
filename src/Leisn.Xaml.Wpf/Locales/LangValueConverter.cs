// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Locales
{
    internal class LangValueConverter : IValueConverter
    {
        private readonly IValueConverter? _converter;
        private readonly object? _parameter;
        private readonly string? _stringFormat;
        public LangValueConverter(IValueConverter? converter, object? parameter, string? stringFormat)
        {
            _converter = converter;
            _parameter = parameter;
            _stringFormat = stringFormat;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object v = value;
            if (_converter != null)
                v = _converter.Convert(value, targetType, parameter, culture);
            if (!string.IsNullOrEmpty(_stringFormat) && v is IFormattable formattable)
                v = formattable.ToString(_stringFormat, null);
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
