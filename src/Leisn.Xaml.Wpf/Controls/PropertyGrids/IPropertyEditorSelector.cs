// By Leisn (https://leisn.com , https://github.com/leisn)

using System.ComponentModel;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids
{
    public interface IPropertyEditorSelector
    {
        IPropertyEditor CreateEditor(PropertyDescriptor propertyDescriptor);
    }
}
