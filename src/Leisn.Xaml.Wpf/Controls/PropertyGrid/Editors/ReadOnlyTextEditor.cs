using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ReadOnlyTextEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
            => new TextBox { IsReadOnly = true };

        public DependencyProperty GetBindingProperty()
            => TextBlock.TextProperty;
    }
}
