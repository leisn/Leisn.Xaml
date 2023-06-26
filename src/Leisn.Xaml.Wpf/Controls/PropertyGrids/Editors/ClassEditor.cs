// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ClassEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var pg = new PropertyGrid { Style = (Style)Application.Current.FindResource("SubPropertyGridBaseStyle") };
            PropertyItem.SetUseExpanderStyle(pg, true);
            return pg;
        }

        public DependencyProperty GetBindingProperty()
        {
            return PropertyGrid.SourceProperty;
        }
    }
}
