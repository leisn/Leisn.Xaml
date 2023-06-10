using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NumericFormatAttribute : Attribute
    {
        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; } = string.Empty;
        /// <summary>
        /// 小数位数
        /// </summary>
        public int Decimals { get; set; } = -1;

        public NumericFormatAttribute() { }
        public NumericFormatAttribute(string unit, int decimals = -1)
        {
            Decimals = decimals;
            Unit = unit;
        }
    }
}
