using Leisn.Xaml.Wpf.Controls;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
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
                vstring = "0";
            if (parameter is not NumericFormat format)
            {
                if (double.TryParse(vstring, out var val1))
                    return val1;
                return DependencyProperty.UnsetValue;
            }

            if (!string.IsNullOrEmpty(format.Suffix))
            {
                var set = new HashSet<char> { ' ' };
                for (int i = 0; i < format.Suffix.Length; i++)
                {
                    var ch = format.Suffix[i];
                    if (!char.IsDigit(ch))
                    {
                        set.Add(ch);
                    }
                }
                Regex regex = new($"[{string.Join("", set)}]", RegexOptions.IgnoreCase);
                var match = regex.Match(vstring);
                if (match.Success)
                {
                    vstring = vstring[..match.Index].Trim();
                }
            }

            if (double.TryParse(vstring, out var val))
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
