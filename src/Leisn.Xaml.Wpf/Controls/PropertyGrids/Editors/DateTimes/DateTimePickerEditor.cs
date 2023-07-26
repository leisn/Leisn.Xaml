// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

using Leisn.Common.Attributes;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class DateTimePickerEditor : IPropertyEditor
    {
        public FrameworkElement CreateElement(PropertyItem item)
        {
            FrameworkElement picker;
            DateTimeSelectionMode mode = DateTimeSelectionMode.DateTime;
            if (item.Attributes.Attr<DateTimePickAttribute>() is DateTimePickAttribute kind)
            {
                mode = kind.SelectionMode;
            }
            picker = new DateTimePicker { SelectionMode = mode };
            ControlAttach.SetShowClear(picker, item.PropertyType == typeof(DateTime?));
            return picker;
        }

        public DependencyProperty GetBindingProperty()
        {
            return DateTimePicker.SelectedDateTimeProperty;
        }
    }
}
