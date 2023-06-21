// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

using Leisn.Common.Media;

namespace Leisn.Xaml.Wpf.Converters
{
    public class ColorSolidBurshConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color color)
            {
                return new SolidColorBrush(color);
            }

            if (value is Hsv hsv)
            {
                return new SolidColorBrush(hsv.ToColor());
            }

            if (value is Rgb rgb)
            {
                return new SolidColorBrush(rgb.ToColor());
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush brush)
            {
                if (targetType == typeof(Color))
                {
                    return brush.Color;
                }

                if (targetType == typeof(Hsv))
                {
                    return brush.Color.ToHsv();
                }

                if (targetType == typeof(Rgb))
                {
                    return brush.Color.ToRgb();
                }
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
