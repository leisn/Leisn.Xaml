// By Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NumericFormatAttribute : Attribute
    {
        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; } = string.Empty;
        /// <summary>
        /// 小数位数
        /// </summary>
        public int Decimals { get; set; } = -1;

        public NumericFormatAttribute() { }
        public NumericFormatAttribute(string suffix, int decimals = -1)
        {
            Decimals = decimals;
            Suffix = suffix;
        }
    }
}
