// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Leisn.Common.Attributes;
using Leisn.Common.Data;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ComboDataEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            Type providerType = item.PropertyDescriptor.Attr<DataProviderAttribute>()!.ProviderType;
            object? instance = AppIoc.GetRequired(providerType);
            if (instance is not IDataProvider<object> provider)
            {
                throw new InvalidCastException($"{providerType} is not IDataProvider.");
            }
            bool isNullable = item.PropertyType.IsClassOrNullable();
            IEnumerable<object> values = provider.GetData();
            Type dataType = provider.GetDataType();
            if (values is not IEnumerable<IDataDeclaration<object>> data)
            {
                if (dataType.IsArray)
                {
                    data = values.Select(x =>
                    {
                        Array array = (Array)x;
                        object? value = array.GetValue(0);
                        return new DataDeclaration
                        {
                            Value = value,
                            DisplayName = array.Length > 1 ? array.GetValue(1)?.ToString() : value?.ToString(),
                            Description = array.Length > 2 ? array.GetValue(2)?.ToString() : null
                        };
                    });
                }
                else
                {
                    data = values.Select(x => new DataDeclaration
                    {
                        Value = x,
                        DisplayName = x.ToString(),
                    });
                }
            }
            ComboBox box = EditorHelper.CreateComboBox(data);
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
