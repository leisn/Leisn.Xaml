using System;

namespace Leisn.Xaml.Wpf.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StringFormatAttribute : Attribute
    {
        public string Format { get; }
        public StringFormatAttribute(string format)
        {
            Format = format;
        }
    }
}
