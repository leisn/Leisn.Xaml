// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class ComboCollecitonEditor : IPropertyEditor
    {
        public bool UseExpanderStyle => true;

        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new StringItemsView
            {
                Padding = new Thickness(5, 0, 5, 5),
                IsCoerceReadOnly = item.IsReadOnly || !item.PropertyType.IsAssignableTo(typeof(ICollection<string>)),
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return StringItemsView.ItemsSourceProperty;
        }
    }
}
