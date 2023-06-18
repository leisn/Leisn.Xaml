// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    public enum BoolVisibilityConverterMode
    {
        FalseCollapsed,
        FalseHidden,
        TrueCollapsed,
        TrueHidden,
    }

    [ValueConversion(typeof(bool), typeof(Visibility), ParameterType = typeof(BoolVisibilityConverterMode))]
    public class BoolVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumEx.TryParse(parameter, out BoolVisibilityConverterMode mode))
            {
                mode = BoolVisibilityConverterMode.FalseCollapsed;
            }
            bool isTrue = (bool)value;
            return mode switch
            {
                BoolVisibilityConverterMode.FalseCollapsed => isTrue ? Visibility.Visible : Visibility.Collapsed,
                BoolVisibilityConverterMode.FalseHidden => isTrue ? Visibility.Visible : Visibility.Hidden,
                BoolVisibilityConverterMode.TrueCollapsed => isTrue ? Visibility.Collapsed : Visibility.Visible,
                BoolVisibilityConverterMode.TrueHidden => isTrue ? Visibility.Hidden : Visibility.Visible,
                _ => throw new ArgumentException("Parameter not BoolVisibilityConverterMode", nameof(parameter)),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumEx.TryParse(parameter, out BoolVisibilityConverterMode mode))
            {
                mode = BoolVisibilityConverterMode.FalseCollapsed;
            }
            bool isVisible = ((Visibility)value) == Visibility.Visible;
            return mode switch
            {
                BoolVisibilityConverterMode.FalseCollapsed or BoolVisibilityConverterMode.FalseHidden => isVisible,
                BoolVisibilityConverterMode.TrueCollapsed or BoolVisibilityConverterMode.TrueHidden => !isVisible,
                _ => throw new ArgumentException("Parameter not BoolVisibilityConverterMode", nameof(parameter)),
            };
        }
    }
}
