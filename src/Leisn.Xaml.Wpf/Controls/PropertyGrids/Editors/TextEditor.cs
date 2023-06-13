// By Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Attributes;

using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids.Editors
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
