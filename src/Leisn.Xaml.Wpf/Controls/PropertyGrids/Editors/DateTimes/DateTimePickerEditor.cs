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
            if (item.Attributes.Attr<DateTimePickAttribute>() is DateTimePickAttribute kind)
            {
                switch (kind.DateTimeType)
                {
                    case DateTimeType.DateTime:
                        picker = new DateTimePicker();
                        goto done;
                    case DateTimeType.TimeOnly:
                        picker = new TimeSelector();
                        goto done;
                    case DateTimeType.DateOnly:
                    default:
                        break;
                }
            }
            picker = new DatePicker();
        done:
            ControlAttach.SetShowClear(picker, item.PropertyType == typeof(DateTime?));
            return picker;
        }

        public DependencyProperty GetBindingProperty()
        {
            return DatePicker.SelectedDateProperty;
        }
    }
}
