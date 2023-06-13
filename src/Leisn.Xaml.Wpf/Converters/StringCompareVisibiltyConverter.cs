// By Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    /// <summary>
    /// Value compare to parameter, return Visibility.
    /// Use ToString() to compare.
    /// Use !parameter to get reversed result.
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(string))]
    public class StringCompareVisibiltyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string? vstr = value?.ToString();
            string? pstr = parameter?.ToString();
            bool equals = string.Equals(vstr, pstr);
            if (pstr?.StartsWith('!') == true)
            {
                pstr = pstr[1..];
                equals = !string.Equals(vstr, pstr);
            }
            return equals ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
