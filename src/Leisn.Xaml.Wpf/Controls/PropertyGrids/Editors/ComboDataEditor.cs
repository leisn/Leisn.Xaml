// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Leisn.Common.Attributes;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ComboDataEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var type = item.PropertyDescriptor.Attr<DataProviderAttribute>()!.ProviderType;
            var datas = EditorHelper.ResolveDataProvider(type);
            ComboBox box = EditorHelper.CreateComboBox(datas);
            box.IsEditable = false; //Cannot edit
            box.IsReadOnly = item.IsReadOnly;
            return box;
        }

        public DependencyProperty GetBindingProperty()
        {
            return Selector.SelectedValueProperty;
        }
    }
}
