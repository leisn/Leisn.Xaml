// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Converters
{
    [ValueConversion(typeof(IConvertible), typeof(IConvertible), ParameterType = typeof(string))]
    public class NumberOperateConverter : IValueConverter
    {
        readonly static char[] Opreations = new char[] { '+', '-', '*', '/', '%', '~', '&', '|', '^' };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var opr = parameter?.ToString()?.Trim();
            if (string.IsNullOrEmpty(opr))
            {
                return value;
            }
            if (value is not IConvertible cv)
            {
                throw new NotSupportedException();
            }
            char opreation = opr[0];
            if (!Opreations.Contains(opreation))
                throw new ArgumentException("Parameter operation not supported.", nameof(parameter));
            var targetStr = opr[1..].Trim();
            if (opreation == '+' || opreation == '-' || opreation == '*' || opreation == '/' || opreation == '%')
            {
                double target = double.Parse(targetStr);
                double dValue = cv.ToDouble(null);
                switch (opreation)
                {
                    case '+':
                        dValue += target;
                        break;
                    case '-':
                        dValue -= target;
                        break;
                    case '*':
                        dValue *= target;
                        break;
                    case '/':
                        dValue /= target;
                        break;
                    case '%':
                        dValue %= target;
                        break;
                }
                return ConvertValue(dValue, targetType);
            }
            else if (opreation == '&' || opreation == '|' || opreation == '^')
            {
                int target = int.Parse(targetStr);
                int iValue = cv.ToInt32(null);
                switch (opreation)
                {
                    case '&':
                        iValue &= target;
                        break;
                    case '|':
                        iValue |= target;
                        break;
                    case '^':
                        iValue ^= target;
                        break;
                }
                return ConvertValue(iValue, targetType);
            }
            else if (opreation == '~')
            {
                if (!string.IsNullOrEmpty(targetStr))
                    throw new ArgumentException($"{parameter} is not correct, for '~', only '~' ", nameof(parameter));
                int iValue = cv.ToInt32(null);
                return ConvertValue(~iValue, targetType);
            }
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var opr = parameter?.ToString()?.Trim();
            if (string.IsNullOrEmpty(opr))
            {
                return value;
            }
            if (value is not IConvertible cv)
            {
                throw new NotSupportedException();
            }
            char opreation = opr[0];
            if (!Opreations.Contains(opreation))
                throw new ArgumentException("Parameter operation not supported.", nameof(parameter));
            if (opreation == '%' || opreation == '&' || opreation == '|')
            {
                throw new NotSupportedException($"Convert back '{opreation}' is not supported.");
            }
            var targetStr = opr[1..].Trim();
            if (opreation == '+' || opreation == '-' || opreation == '*' || opreation == '/')
            {
                double target = double.Parse(targetStr);
                double dValue = cv.ToDouble(null);
                switch (opreation)
                {
                    case '+':
                        dValue -= target;
                        break;
                    case '-':
                        dValue += target;
                        break;
                    case '*':
                        dValue /= target;
                        break;
                    case '/':
                        dValue *= target;
                        break;
                }
                return ConvertValue(dValue, targetType);
            }
            else if (opreation == '^')
            {
                int target = int.Parse(targetStr);
                int iValue = cv.ToInt32(null) ^ target;
                return ConvertValue(iValue, targetType);
            }
            else if (opreation == '~')
            {
                if (!string.IsNullOrEmpty(targetStr))
                    throw new ArgumentException($"{parameter} is not correct, for '~', only '~' ", nameof(parameter));
                int iValue = ~cv.ToInt32(null);
                return ConvertValue(iValue, targetType);
            }
            return DependencyProperty.UnsetValue;
        }

        private static object ConvertValue(object dValue, Type targetType)
        {
            var dV = (IConvertible)dValue;
            var code = Type.GetTypeCode(targetType);
            return code switch
            {
                TypeCode.Boolean => dV.ToBoolean(null),
                TypeCode.Char => dV.ToChar(null),
                TypeCode.SByte => dV.ToSByte(null),
                TypeCode.Byte => dV.ToByte(null),
                TypeCode.Int16 => dV.ToInt16(null),
                TypeCode.UInt16 => dV.ToUInt16(null),
                TypeCode.Int32 => dV.ToInt32(null),
                TypeCode.UInt32 => dV.ToUInt32(null),
                TypeCode.Int64 => dV.ToInt64(null),
                TypeCode.UInt64 => dV.ToUInt64(null),
                TypeCode.Single => dV.ToSingle(null),
                TypeCode.Double => dV.ToDouble(null),
                TypeCode.Decimal => dV.ToDecimal(null),
                TypeCode.DateTime => dV.ToDateTime(null),
                _ => dV.ToType(targetType, null)
            };
        }
    }
}
