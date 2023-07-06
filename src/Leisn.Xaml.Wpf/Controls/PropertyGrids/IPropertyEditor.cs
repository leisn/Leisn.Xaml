// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public interface IPropertyEditor
    {
        public bool UseExpanderStyle => false;

        FrameworkElement CreateElement(PropertyItem item);

        DependencyProperty GetBindingProperty();

        public FrameworkElement? GetOperationContent()
        {
            return null;
        }
    }
}
