using Leisn.Common;
using Leisn.Common.Attributes;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class NumberEditor : IPropertyEditor
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Increment { get; set; }
        public NumericType NumericType { get; set; }

        public NumberEditor(double minium, double maximum, double increment, NumericType type)
        {
            Minimum = minium;
            Maximum = maximum;
            Increment = increment;
            NumericType = type;
        }

        public FrameworkElement CreateElement(PropertyItem item)
        {
            PropertyDescriptor propertyDescriptor = item.PropertyDescriptor;
            if (propertyDescriptor.Attr<NumericUpDownAttribute>() is NumericUpDownAttribute attr)
            {
                Maximum = attr.Maximum;
                Minimum = attr.Minimum;
                Increment = attr.Increment;
            }
            if (propertyDescriptor.Attr<RangeAttribute>() is RangeAttribute range)
            {
                Maximum = Convert.ToDouble(range.Maximum);
                Minimum = Convert.ToDouble(range.Minimum);
            }
            if (propertyDescriptor.Attr<IncrementAttribute>()?.Increment is double increment)
            {
                Increment = increment;
            }
            if (Maximum - Minimum < Increment)
            {
                Increment = (Maximum - Minimum) / 10;
            }

            NumericFormat? numberFormat = null;
            if (propertyDescriptor.Attr<NumericFormatAttribute>() is NumericFormatAttribute format)
            {
                numberFormat = new NumericFormat
                {
                    Unit = format.Unit,
                    Decimals = format.Decimals,
                };
            }

            return new NumericUpDown
            {
                IsReadOnly = item.IsReadOnly,
                NumericType = NumericType,
                Minimum = Minimum,
                Maximum = Maximum,
                Increment = Increment,
                Format = numberFormat!
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return System.Windows.Controls.Primitives.RangeBase.ValueProperty;
        }
    }
}
