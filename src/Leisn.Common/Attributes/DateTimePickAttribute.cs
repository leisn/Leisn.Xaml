// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateTimePickAttribute : Attribute
    {
        public DateTimeType DateTimeType { get; }
        public DateTimePickAttribute(DateTimeType type)
        {
            DateTimeType = type;
        }
    }

    public enum DateTimeType
    {
        DateOnly,
        TimeOnly,
        DateTime,
    }
}
