﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    internal class NumericCollectionEditor : CollectionEditorBase<NumericUpDown>
    {
        private NumericEditorParams _params;
        private Type _numberType;
        public NumericCollectionEditor(Type numberType)
        {
            _numberType = numberType;
            _params = EditorHelper.ResolveTypeNumericParams(numberType);
        }

        public override FrameworkElement CreateElement(PropertyItem item)
        {
            _params = EditorHelper.ResolveAttrNumericParams(_params, item.PropertyDescriptor);
            return base.CreateElement(item);
        }

        protected override NumericUpDown CreateItemElement(object? item)
        {
            var control = new NumericUpDown
            {
                Value = Convert.ToDouble(item),
                IsReadOnly = IsCoerceReadOnly,
                NumericType = _params.NumericType,
                Minimum = _params.Minimum,
                Maximum = _params.Maximum,
                Increment = _params.Increment,
                Format = _params.Format
            };
            control.ValueChanged += Control_ValueChanged;
            return control;
        }

        private void Control_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateItemValue((NumericUpDown)sender);
        }

        protected override object CreateNewItem()
        {
            var value = _params.Minimum > 0 ? _params.Minimum : _params.Maximum < 0 ? _params.Maximum : 0;
            return ((IConvertible)value).ToType(_numberType, null);
        }

        protected override object GetElementValue(NumericUpDown element)
        {
            return ((IConvertible)element.Value).ToType(_numberType, null);
        }

        protected override void OnRemoveElement(NumericUpDown element)
        {
            element.ValueChanged -= Control_ValueChanged;
        }
    }
}
