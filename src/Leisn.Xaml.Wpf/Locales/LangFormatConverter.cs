using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Locales
{
    internal struct LangFormat
    {
        public string[] Keys;
        public string Format;
    }

    internal class LangFormatConverter : IValueConverter
    {
        public static LangFormatConverter Instance { get; } = new LangFormatConverter();
        private LangFormatConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dict = (IReadOnlyDictionary<string, string>)value;
            var format = (LangFormat)parameter;
            var values = new string[format.Keys.Length];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = dict[format.Keys[i]];
            }
            return string.Format(format.Format, values);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
