// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    public class MutexVisilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = (Visibility)value;
            return visible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = (Visibility)value;
            return visible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
