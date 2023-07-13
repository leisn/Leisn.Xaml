// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal struct NumericEditorParams
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Increment { get; set; }
        public NumericType NumericType { get; set; }
        public NumericFormat Format { get; set; }

        public NumericEditorParams(double minium, double maximum, double increment, NumericType type)
        {
            Minimum = minium;
            Maximum = maximum;
            Increment = increment;
            NumericType = type;
            Format = new NumericFormat();
        }
    }

    internal class NumericEditor : IPropertyEditor
    {
        private NumericEditorParams _params = new();

        public NumericEditor(NumericEditorParams param)
        {
            _params = param;
        }

        public FrameworkElement CreateElement(PropertyItem item)
        {
            _params = EditorHelper.ResolveAttrNumericParams(_params, item.Attributes);

            return new NumericUpDown
            {
                IsReadOnly = item.IsReadOnly,
                NumericType = _params.NumericType,
                Minimum = _params.Minimum,
                Maximum = _params.Maximum,
                Increment = _params.Increment,
                Format = _params.Format
            };
        }

        public DependencyProperty GetBindingProperty()
        {
            return System.Windows.Controls.Primitives.RangeBase.ValueProperty;
        }
    }
}
