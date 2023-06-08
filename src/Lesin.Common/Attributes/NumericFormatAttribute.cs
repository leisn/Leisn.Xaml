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
        public string? Unit { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public byte Decimals { get; set; }

        public NumericFormatAttribute() { }
        public NumericFormatAttribute(byte decimals, string unit)
        {
            Decimals = decimals;
            Unit = unit;
        }
    }
}
