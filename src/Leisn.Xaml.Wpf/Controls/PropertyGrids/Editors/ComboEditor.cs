using Leisn.Common;
using Leisn.Common.Attributes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ComboEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var attr = (DataProviderAttribute)item.PropertyDescriptor.Attr<DataProviderAttribute>();
            var box = EditorHelper.CreateComboBox(null);
            box.IsReadOnly = item.IsReadOnly;
            return box;
        }

        public DependencyProperty GetBindingProperty()
            => ComboBox.SelectedValueProperty;
    }
}
