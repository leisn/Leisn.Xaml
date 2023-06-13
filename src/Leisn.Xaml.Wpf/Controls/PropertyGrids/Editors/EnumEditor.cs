// By Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Data;
using Leisn.Common.Extensions;

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids.Editors
{
    internal class EnumEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            System.Collections.Generic.IEnumerable<DataDeclaration> values = Enum.GetValues(item.PropertyType).OfType<Enum>().Select(x => new DataDeclaration
            {
                Value = x,
                DisplayName = x.Attr<CategoryAttribute>()?.Category ?? x.ToString(),
                Description = x.Attr<DescriptionAttribute>()?.Description ?? x.ToString()
            });

            ComboBox box = EditorHelper.CreateComboBox(values);
            box.IsReadOnly = item.IsReadOnly;
            return box;
        }

        public DependencyProperty GetBindingProperty()
        {
            return System.Windows.Controls.Primitives.Selector.SelectedValueProperty;
        }
    }
}
