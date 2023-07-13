// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;

namespace Leisn.Xaml.Wpf.Controls
{
    public interface IPropertyEditor
    {
        public bool UseExpanderStyle => false;
        public FrameworkElement? GetOperationContent() { return null; }

        FrameworkElement CreateElement(PropertyItem item);
        DependencyProperty GetBindingProperty();
    }
}
