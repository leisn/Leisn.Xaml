// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;

using Leisn.Xaml.Wpf.Controls;

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
            if (format.Decimals >= 0)
            {
                vstring = string.Format($"{{0:N{format.Decimals}}}", v);
            }
            if (!string.IsNullOrEmpty(format.Suffix))
            {
                vstring += $" {format.Suffix}";
            }
            return vstring;
        }

        //to decimal
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string vstring = value.ToString()!.Trim();
            if (string.IsNullOrEmpty(vstring))
            {
                vstring = "0";
            }

            if (parameter is not NumericFormat format)
            {
                return double.TryParse(vstring, out double val1) ? val1 : DependencyProperty.UnsetValue;
            }

            if (!string.IsNullOrEmpty(format.Suffix))
            {
                HashSet<char> set = new() { ' ' };
                for (int i = 0; i < format.Suffix.Length; i++)
                {
                    char ch = format.Suffix[i];
                    if (!char.IsDigit(ch))
                    {
                        set.Add(ch);
                    }
                }
                Regex regex = new($"[{string.Join("", set)}]", RegexOptions.IgnoreCase);
                Match match = regex.Match(vstring);
                if (match.Success)
                {
                    vstring = vstring[..match.Index].Trim();
                }
            }

            if (double.TryParse(vstring, out double val))
            {
                if (format.Decimals >= 0)
                {
                    val = Math.Round(val, format.Decimals);
                }
                return val;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
