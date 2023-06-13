// By Leisn (https://leisn.com , https://github.com/leisn)

using Leisn.Common.Attributes;
using Leisn.Xaml.Wpf.Controls.Inputs;

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.PropertyGrids.Editors
{
    internal class NumericEditor : IPropertyEditor
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Increment { get; set; }
        public NumericType NumericType { get; set; }

        public NumericEditor(double minium, double maximum, double increment, NumericType type)
        {
            Minimum = minium;
            Maximum = maximum;
            Increment = increment;
            NumericType = type;
        }

        public FrameworkElement CreateElement(PropertyItem item)
        {
            if (Minimum > Maximum)
            {
                throw new InvalidOperationException($"Minimum > Maxium: {Minimum} > {Maximum}");
            }

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

            NumericFormat numberFormat = new();
            if (propertyDescriptor.Attr<NumericFormatAttribute>() is NumericFormatAttribute format)
            {
                numberFormat.Suffix = format.Suffix;
                numberFormat.Decimals = format.Decimals;
            }

            return new NumericUpDown
            {
                IsReadOnly = item.IsReadOnly,
                NumericType = NumericType,
                Minimum = Minimum,
                Maximum = Maximum,
                Increment = Increment,
                Format = numberFormat
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return System.Windows.Controls.Primitives.RangeBase.ValueProperty;
        }
    }
}
