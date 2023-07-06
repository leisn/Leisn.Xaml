using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var year = (int)values[0];
            var month = (int)values[1];
            return DateTime.DaysInMonth(year, month);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
