using Leisn.Common;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class TextEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var element = new TextBox { IsReadOnly = item.IsReadOnly, };
            if (item.PropertyDescriptor.Attr<StringLengthAttribute>() is StringLengthAttribute len)
            {
                element.MaxLength = len.MaximumLength;
            }
            return element;
        }

        public DependencyProperty GetBindingProperty() => TextBox.TextProperty;
    }
}
