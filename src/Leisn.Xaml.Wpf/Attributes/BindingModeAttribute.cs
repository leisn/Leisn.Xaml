using System;
using System.Windows.Data;

namespace Leisn.Xaml.Wpf.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class BindingModeAttribute : Attribute
    {
        public BindingMode Mode { get; }
        public BindingModeAttribute(BindingMode mode)
        {
            Mode = mode;
        }
    }
}
