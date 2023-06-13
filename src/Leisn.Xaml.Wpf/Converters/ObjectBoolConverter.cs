// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    [ValueConversion(typeof(object), typeof(bool), ParameterType = typeof(Nullable))]
    public class ObjectBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value is null || (value is string str && string.IsNullOrEmpty(str));
            return parameter is not null ? !isNull : isNull;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
