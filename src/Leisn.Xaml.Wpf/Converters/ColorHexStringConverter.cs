// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Converters
{
    [ValueConversion(typeof(Color), typeof(string), ParameterType = typeof(Nullable))]
    public class ColorHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)value;
            return color.ToHex(parameter is not null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string hex = (string)value;
            try
            {
                return ColorEx.FromHex(hex);
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
