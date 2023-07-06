using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

using Leisn.Common.Attributes;

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
