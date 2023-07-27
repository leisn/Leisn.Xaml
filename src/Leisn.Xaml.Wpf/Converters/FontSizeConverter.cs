// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    public class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double mutipy = ParseParam(parameter);
            double v = (double)value;
            return mutipy * v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double mutipy = ParseParam(parameter);
            double v = (double)value;
            return v / mutipy;
        }

        private static double ParseParam(object parameter)
        {
            double mutipy = 1;
            if (parameter is not null)
            {
                string pStr = parameter.ToString()!.Trim();
                if (pStr.EndsWith('%'))
                {
                    pStr = pStr.Substring(0, Math.Max(0, pStr.Length - 1));
                    if (double.TryParse(pStr, out double precent))
                    {
                        mutipy = precent / 100;
                    }
                }
                else
                {
                    _ = double.TryParse(pStr, out mutipy);
                }
            }
            return mutipy;
        }
    }
}
