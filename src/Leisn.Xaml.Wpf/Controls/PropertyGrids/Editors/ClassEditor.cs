// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ClassEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new PropertyGrid() { Style = (Style)item.FindResource("SubPropertyGridBaseStyle") };
        }

        public DependencyProperty GetBindingProperty()
        {
            return PropertyGrid.SourceProperty;
        }

        public bool UseExpanderStyle => true;
    }
}
