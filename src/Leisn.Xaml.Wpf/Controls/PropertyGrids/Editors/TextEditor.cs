// @Leisn (https://leisn.com , https://github.com/leisn)

using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Locales;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class TextEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            TextBox element = new() { IsReadOnly = item.IsReadOnly, };
            if (item.PropertyDescriptor.Attr<StringLengthAttribute>() is StringLengthAttribute len)
            {
                element.MaxLength = len.MaximumLength;
            }
            if (item.PropertyDescriptor.Attr<PlaceholderAttribute>() is PlaceholderAttribute p)
            {
                element.SetBindingLangFormat(ControlAttach.PlaceholderProperty, p.Placeholder);
            }
            return element;
        }

        public DependencyProperty GetBindingProperty()
        {
            return TextBox.TextProperty;
        }
    }
}
