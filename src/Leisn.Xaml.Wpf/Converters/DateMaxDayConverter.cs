// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    /// <summary>
    /// [Year,Month] => Days in month
    /// </summary>
    public class DateMaxDayConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int year = (int)values[0];
            int month = (int)values[1];
            return DateTime.DaysInMonth(year, month);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
