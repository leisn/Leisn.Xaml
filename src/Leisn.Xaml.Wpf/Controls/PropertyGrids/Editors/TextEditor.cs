using Leisn.Common;
using Leisn.Common.Attributes;

using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

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
            return element;
        }

        public DependencyProperty GetBindingProperty()
        {
            return TextBox.TextProperty;
        }
    }
}
