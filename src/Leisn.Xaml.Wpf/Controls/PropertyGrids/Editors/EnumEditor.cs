// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class EnumEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var datas = EditorHelper.ResolveDataProvider(item.PropertyType);
            ComboBox box = EditorHelper.CreateComboBox(datas);
            box.IsReadOnly = item.IsReadOnly;
            return box;
        }

        public DependencyProperty GetBindingProperty()
        {
            return System.Windows.Controls.Primitives.Selector.SelectedValueProperty;
        }
    }
}
