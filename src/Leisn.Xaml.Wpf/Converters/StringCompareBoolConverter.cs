using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    /// <summary>
    /// Value compare to parameter, return bool.
    /// Use ToString() to compare.
    /// Use !parameter to get reversed result.
    /// </summary>
    [ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(string))]
    public class StringCompareBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var vstr = value?.ToString();
            var pstr = parameter?.ToString();
            if (pstr?.StartsWith('!') == true)
            {
                pstr = pstr.Substring(1);
                return !string.Equals(vstr, pstr);
            }
            return string.Equals(vstr, pstr);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
