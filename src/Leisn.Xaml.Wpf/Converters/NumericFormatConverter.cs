using Leisn.Xaml.Wpf.Controls;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    [ValueConversion(typeof(double), typeof(string), ParameterType = typeof(NumericFormat))]
    internal class NumericFormatConverter : IValueConverter
    {
        public static NumericFormatConverter Instance { get; } = new NumericFormatConverter();
        //to string
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string vstring = value.ToString()!;
            if (parameter is not NumericFormat format)
            {
                return vstring;
            }

            double v = (double)value;
            vstring = string.Format($"{{0:N{format.Decimals}}}", v);
            if (format.Unit != null)
            {
                vstring += $" {format.Unit}";
            }
            return vstring;
        }

        //to decimal
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string vstring = value.ToString()!;
            if (parameter is NumericFormat format && format.Unit != null)
            {
                int index = vstring.IndexOf(format.Unit);
                if (index > -1)
                {
                    vstring = vstring.Remove(index).Trim();
                }
            }
            return double.Parse(vstring);
        }
    }
}
