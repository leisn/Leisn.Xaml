// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class TimeSelectorEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new TimeSelector() { IsEnabled = !item.IsReadOnly };
        }

        public DependencyProperty GetBindingProperty()
        {
            return TimeSelector.TimeProperty;
        }
    }

    internal class DateSelectorEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new DateSelector() { IsEnabled = !item.IsReadOnly };
        }

        public DependencyProperty GetBindingProperty()
        {
            return DateSelector.DateProperty;
        }
    }
}
