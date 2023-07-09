// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    /// <summary>
    /// 圆形边框CornerRadius转换，values={widht,height}
    /// </summary>
    [ValueConversion(typeof(double[]), typeof(CornerRadius))]
    public class BorderCircleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is double width && values[1] is double height)
            {
                if (width < double.Epsilon || height < double.Epsilon)
                {
                    return new CornerRadius();
                }
                double min = Math.Min(width, height);
                return new CornerRadius((int)(min / 2));
            }
            return DependencyProperty.UnsetValue;

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
