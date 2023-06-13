
using System;
using System.Collections.Generic;

namespace Leisn.Common.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// 解析格式化字符串，例如 "{key} anything {value}" 返回 [(0,4,key),(15,21,value)]
        /// </summary>
        public static List<(int Start, int End, string Value)> ParseFormat(string format)
        {
            List<(int, int, string)> indexs = new();
            int start = -1;
            for (int i = 0; i < format.Length; i++)
            {
                var ch = format[i];
                if (ch == '{')
                {
                    start = i;
                }
                else if (ch == '}')
                {
                    if (start < 0) //前无{
                        continue;
                    indexs.Add((start, i, format[(start + 1)..i]));
                    start = -1;
                }
            }
            return indexs;
        }

        /// <summary>
        /// 解析格式化字符串，例如 "{key} anything {value}" 返回 [key,value],convertedFormat="{0} anything {1}"
        /// </summary>
        public static string[] ParseFormat(string format, out string convertedFormat)
        {
            convertedFormat = format;
            var indexs = ParseFormat(convertedFormat);
            var keys = new string[indexs.Count];
            for (int i = indexs.Count - 1; i >= 0; i--)
            {
                var (start, end, value) = indexs[i];
                keys[i] = value;
                convertedFormat = convertedFormat.Replace(start, end, $"{{{i}}}");
            }
            return keys;
        }

        public static string Format(string format, Dictionary<string, object> values)
        {
            var keys = ParseFormat(format, out var convertedFormat);
            object[] vs = new object[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                vs[i] = values[keys[i]];
            }
            return string.Format(convertedFormat, vs);
        }

        public static string Format(string format, object[] values)
        {
            ParseFormat(format, out var convertedFormat);
            return string.Format(convertedFormat, values);
        }

    }
}
