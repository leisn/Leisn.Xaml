﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Converters
{
    [ValueConversion(typeof(Color), typeof(string), ParameterType = typeof(Nullable))]
    public class ColorHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (Color)value;
            return color.ToHex(parameter is not null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var hex = (string)value;
            return ColorEx.FromHex(hex);
        }
    }
}
