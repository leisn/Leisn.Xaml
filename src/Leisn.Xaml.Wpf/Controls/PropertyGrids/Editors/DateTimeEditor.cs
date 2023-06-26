// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class DateTimeEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new DatePicker();
        }

        public DependencyProperty GetBindingProperty()
        {
            return DatePicker.SelectedDateProperty;
        }
    }
}
