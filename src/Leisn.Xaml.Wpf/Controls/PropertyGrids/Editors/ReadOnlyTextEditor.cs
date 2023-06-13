// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ReadOnlyTextEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new TextBox { IsReadOnly = true };
        }

        public DependencyProperty GetBindingProperty()
        {
            return TextBlock.TextProperty;
        }
    }
}
