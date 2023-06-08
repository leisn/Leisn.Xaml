using System;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ValueConverterAttribute : Attribute
    {
        public Type Type { get; }
        public ValueConverterAttribute(Type type)
        {
            Type = type;
            if (!type.IsSubclassOf(typeof(IValueConverter)))
            {
                throw new ArgumentException($"{type.FullName} is not IValueConverter");
            }
        }
    }
}
