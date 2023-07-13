// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;

namespace Leisn.Xaml.Wpf.Controls
{
    public interface IPropertyEditorSelector
    {
        public IPropertyEditor CreateEditor(PropertyDescriptor propertyDescriptor)
        {
            return CreateEditor(propertyDescriptor.PropertyType, propertyDescriptor.Attributes);
        }

        IPropertyEditor CreateEditor(Type propertyType, AttributeCollection propertyAttributes);
    }
}
