// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using Leisn.Xaml.Wpf.Converters;

namespace Leisn.Xaml.Wpf.Controls
{
    public struct NumericFormat
    {
        /// <summary>
        /// 后缀
        /// </summary>
        public string Suffix { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int Decimals { get; set; }
        public NumericFormat()
        {
            Suffix = string.Empty;
            Decimals = -1;
        }
    }


    [TemplatePart(Name = TextboxName, Type = typeof(TextBox))]
    [TemplatePart(Name = ButtonIncreaseName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = ButtonMinusName, Type = typeof(ButtonBase))]
    public class NumericUpDown : RangeBase
    {
        private const string ButtonIncreaseName = "PART_Increase";
        private const string ButtonMinusName = "PART_Minus";
        private const string TextboxName = "PART_TextBox";
        private TextBox _textBox = null!;
        private ButtonBase _increaseButton = null!;
        private ButtonBase _minusButton = null!;

        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public NumericType NumericType
        {
            get => (NumericType)GetValue(NumericTypeProperty);
            set => SetValue(NumericTypeProperty, value);
        }
        public static readonly DependencyProperty NumericTypeProperty =
            NumericBox.NumericTypeProperty.AddOwner(typeof(NumericUpDown),
                new FrameworkPropertyMetadata(NumericType.Int, null, new CoerceValueCallback(CoerceNumericType)));

        private static object CoerceNumericType(DependencyObject d, object baseValue)
        {
            NumericUpDown control = (NumericUpDown)d;
            NumericType value = (NumericType)baseValue;
            if (control.Minimum < 0)
            {
                if (value == NumericType.UInt)
                {
                    value = NumericType.Int;
                }
                else if (value == NumericType.UFloat)
                {
                    value = NumericType.Float;
                }
            }
            else
            {
                if (value == NumericType.Int)
                {
                    value = NumericType.UInt;
                }
                else if (value == NumericType.Float)
                {
                    value = NumericType.UFloat;
                }
            }
            return value;
        }

        public NumericFormat Format
        {
            get => (NumericFormat)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(NumericFormat), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(new NumericFormat(), new PropertyChangedCallback(OnFormatChanged)));

        private static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumericUpDown box = (NumericUpDown)d;
            box.OnFormatChanged();
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));

        public bool IsEditing => (bool)GetValue(IsEditingProperty);
        internal static readonly DependencyPropertyKey IsEditingPropertyKey =
            DependencyProperty.RegisterReadOnly("IsEditing", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(false));
        public static readonly DependencyProperty IsEditingProperty = IsEditingPropertyKey.DependencyProperty;

        public double Increment
        {
            get => (double)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }
        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register("Increment", typeof(double), typeof(NumericUpDown), new PropertyMetadata(0d));

        internal void SetIsEditing(bool value)
        {
            SetValue(IsEditingPropertyKey, value);
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            NumericType = (NumericType)CoerceNumericType(this, NumericType);
        }

        //protected override void OnValueChanged(double oldValue, double newValue)
        //{
        //    base.OnValueChanged(oldValue, newValue);
        //    if (_minusButton == null)
        //        return;
        //    _minusButton.Visibility = newValue <= Minimum ? Visibility.Collapsed : Visibility.Visible;
        //    _increaseButton.Visibility = newValue >= Maximum ? Visibility.Collapsed : Visibility.Visible;
        //    var corner = new CornerRadius();
        //    if (newValue <= Minimum)
        //    {
        //        corner.TopLeft = 4;
        //        corner.BottomLeft = 4;
        //    }
        //    if (newValue >= Maximum)
        //    {
        //        corner.TopRight = 4;
        //        corner.BottomRight = 4;
        //    }
        //    BorderAttach.SetCornerRadius(_textBox, corner);
        //}

        public override void OnApplyTemplate()
        {
            if (_increaseButton != null)
            {
                _increaseButton.Click -= IncreaseValue;
            }

            if (_minusButton != null)
            {
                _minusButton.Click -= MinusValue;
            }

            if (_textBox != null)
            {
                _textBox.GotFocus -= TextBox_GotFocus;
                _textBox.LostFocus -= TextBox_LostFocus;
            }

            base.OnApplyTemplate();
            _textBox = (TextBox)GetTemplateChild(TextboxName);
            _increaseButton = (ButtonBase)GetTemplateChild(ButtonIncreaseName);
            _minusButton = (ButtonBase)GetTemplateChild(ButtonMinusName);

            _increaseButton.Click += IncreaseValue;
            _minusButton.Click += MinusValue;
            _textBox.GotFocus += TextBox_GotFocus;
            _textBox.LostFocus += TextBox_LostFocus;

            OnFormatChanged();
        }

        #region focus handler

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetIsEditing(false);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SetIsEditing(true);
        }
        #endregion

        private void OnFormatChanged()
        {
            if (_textBox == null)
            {
                return;
            }

            BindingOperations.ClearBinding(_textBox, TextBox.TextProperty);
            Binding binding = new("Value")
            {
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = NumericFormatConverter.Instance,
                ConverterParameter = Format
            };
            _ = BindingOperations.SetBinding(_textBox, TextBox.TextProperty, binding);
        }


        private void IncreaseValue(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            Value += Increment;
        }

        private void MinusValue(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            Value -= Increment;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsEditing)
                return;
            e.Handled = true;
            switch (e.Key)
            {
                case Key.Left:
                    Value -= Increment;
                    break;
                case Key.Right:
                    Value += Increment;
                    break;
                case Key.Up:
                    Value -= SmallChange;
                    break;
                case Key.Down:
                    Value += SmallChange;
                    break;
                case Key.PageUp:
                    Value -= LargeChange;
                    break;
                case Key.PageDown:
                    Value += LargeChange;
                    break;
                case Key.Home:
                    Value = Minimum;
                    break;
                case Key.End:
                    Value = Maximum;
                    break;
                default:
                    e.Handled = false;
                    base.OnKeyDown(e);
                    break;
            }

        }

    }
}
