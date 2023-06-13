// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    public enum ObjectVisibilityConverterMode
    {
        NullCollapsed,
        NullHidden,
        NotNullCollapsed,
        NotNullHidden,
    }

    [ValueConversion(typeof(object), typeof(Visibility), ParameterType = typeof(ObjectVisibilityConverterMode))]
    public class ObjectVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumEx.TryParse<ObjectVisibilityConverterMode>(parameter, out var mode))
                mode = ObjectVisibilityConverterMode.NullCollapsed;

            bool isNull = value is null || (value is string str && string.IsNullOrEmpty(str));
            return mode switch
            {
                ObjectVisibilityConverterMode.NullCollapsed => isNull ? Visibility.Collapsed : Visibility.Visible,
                ObjectVisibilityConverterMode.NullHidden => isNull ? Visibility.Hidden : Visibility.Visible,
                ObjectVisibilityConverterMode.NotNullCollapsed => isNull ? Visibility.Visible : Visibility.Collapsed,
                ObjectVisibilityConverterMode.NotNullHidden => isNull ? Visibility.Visible : Visibility.Hidden,
                _ => throw new ArgumentException("Parameter not ObjectVisibilityConverter", nameof(parameter)),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
