// By Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Extensions;

using System;
using System.Globalization;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    /// <summary>
    /// MultiBoolToBoolConverter parameter for compare mode
    /// </summary>
    public enum MultiBoolToBoolMode
    {
        /// <summary>
        /// 全部True，返回True
        /// </summary>
        And,
        /// <summary>
        /// 任意True，返回True
        /// </summary>
        Or,
        /// <summary>
        /// 全部False,返回True
        /// </summary>
        AndFalse,
        /// <summary>
        /// 任意Flase，返回True
        /// </summary>
        OrFalse,
        /// <summary>
        /// 全部True，返回false
        /// </summary>
        AddReverse,
        /// <summary>
        /// 任意Ture，返回false
        /// </summary>
        OrReverse,
    }

    /// <summary>
    /// 多bool转bool
    /// </summary>
    [ValueConversion(typeof(bool[]), typeof(bool), ParameterType = typeof(MultiBoolToBoolMode))]
    public class MultiBoolToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (!EnumEx.TryParse<MultiBoolToBoolMode>(parameter, out MultiBoolToBoolMode mode))
            {
                mode = MultiBoolToBoolMode.And;
            }

            bool result = (bool)values[0];

            switch (mode)
            {
                case MultiBoolToBoolMode.And:
                    if (result == true)
                    {
                        for (int i = 1; i < values.Length; i++)
                        {
                            if ((bool)values[i] == false)
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                    break;
                case MultiBoolToBoolMode.Or:
                    if (result == false)
                    {
                        for (int i = 1; i < values.Length; i++)
                        {
                            if ((bool)values[i] == true)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    break;
                case MultiBoolToBoolMode.AndFalse:
                    if (result == false)
                    {
                        for (int i = 1; i < values.Length; i++)
                        {
                            if ((bool)values[i] != false)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    result = !result;
                    break;
                case MultiBoolToBoolMode.OrFalse:
                    if (result != false)
                    {
                        for (int i = 1; i < values.Length; i++)
                        {
                            if ((bool)values[i] == false)
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                    result = !result;
                    break;
                case MultiBoolToBoolMode.AddReverse:
                    for (int i = 1; i < values.Length; i++)
                    {
                        result = result && (bool)values[i];
                    }
                    result = result != true;
                    break;
                case MultiBoolToBoolMode.OrReverse:
                    if (result == false)
                    {
                        for (int i = 1; i < values.Length; i++)
                        {
                            if ((bool)values[i] == true)
                            {
                                result = true;
                                break;
                            }
                        }
                    }
                    result = !result;
                    break;
                default:
                    throw new InvalidOperationException($"Not supported mode: {parameter}");
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
