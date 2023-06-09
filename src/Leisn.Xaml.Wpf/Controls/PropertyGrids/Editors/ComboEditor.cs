using Leisn.Common;
using Leisn.Common.Attributes;
using Leisn.Common.Interfaces;
using Leisn.Common.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ComboEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            var providerType = item.PropertyDescriptor.Attr<DataProviderAttribute>()!.ProviderType;
            var instance = Activator.CreateInstance(providerType);
            if (instance is not IDataProvider<object> provider)
                throw new InvalidCastException($"{providerType} is not IDataProvider");
            var values = provider.GetData();
            var dataType = provider.GetDataType();

            if (values is not IEnumerable<IDataDeclaration<object>> data)
            {
                if (dataType.IsArray)
                {
                    data = values.Select(x =>
                    {
                        var array = (Array)x;
                        var value = array.GetValue(0);
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
                        DisplayName = x?.ToString(),
                    });
                }
            }
            var box = EditorHelper.CreateComboBox(data);
            box.IsReadOnly = item.IsReadOnly;
            return box;
        }

        public DependencyProperty GetBindingProperty() => ComboBox.SelectedValueProperty;
    }
}
