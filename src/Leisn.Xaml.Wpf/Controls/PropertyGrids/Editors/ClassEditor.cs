// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ClassEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            PropertyGrid pg = new() { Style = (Style)item.FindResource("SubPropertyGridBaseStyle") };
            PropertyItem.SetUseExpanderStyle(pg, true);
            return pg;
        }

        public DependencyProperty GetBindingProperty()
        {
            return PropertyGrid.SourceProperty;
        }
    }
}
