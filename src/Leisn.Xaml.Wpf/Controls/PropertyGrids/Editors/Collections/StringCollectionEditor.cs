// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Collections.Generic;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class StringCollectionEditor : IPropertyEditor
    {
        public bool UseExpanderStyle => true;

        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new StringItemsView
            {
                Padding = new Thickness(5, 0, 5, 5),
                IsCoerceReadOnly = item.IsReadOnly,
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return StringItemsView.ItemsSourceProperty;
        }
    }
}
