// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PlaceholderAttribute : Attribute
    {
        public string Placeholder { get; }

        public PlaceholderAttribute(string placeholder)
        {
            Placeholder = placeholder;
        }
    }
}
