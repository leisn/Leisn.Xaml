using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids
{
    public interface IPropertyEditorSelector
    {
        IPropertyEditor CreateEditor(PropertyDescriptor propertyDescriptor);
    }
}
