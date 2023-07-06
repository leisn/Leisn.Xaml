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
    internal class TimePickerEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            return new TimePicker() { IsEnabled = !item.IsReadOnly };
        }

        public DependencyProperty GetBindingProperty()
        {
            return TimePicker.TimeProperty;
        }
    }
}
