// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateTimePickAttribute : Attribute
    {
        public DateTimeSelectionMode SelectionMode { get; }

        public DateTimePickAttribute(DateTimeSelectionMode mode)
        {
            SelectionMode = mode;
        }
    }

    public enum DateTimeSelectionMode
    {
        DateTime,
        DateOnly,
        TimeOnly,
    }
}
