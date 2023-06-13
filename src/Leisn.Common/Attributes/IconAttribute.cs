// By Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IconAttribute : Attribute
    {
        public string IconName { get; }
        public IconAttribute(string iconName)
        {
            IconName = iconName;
        }
    }
}
