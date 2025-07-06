// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = "PART_Thumb", Type = typeof(Border))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class RangeBox : RangeBase
    {
        private const int MaxDecimals = 15;
        private Border thumb = null!;
        private TextBox textBox = null!;
        static RangeBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeBox), new FrameworkPropertyMetadata(typeof(RangeBox)));
        }
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(RangeBox),
                new FrameworkPropertyMetadata(""));

        public Brush ThumbBackground
        {
            get => (Brush)GetValue(ThumbBackgroundProperty);
            set => SetValue(ThumbBackgroundProperty, value);
        }

        public static readonly DependencyProperty ThumbBackgroundProperty =
            DependencyProperty.Register("ThumbBackground", typeof(Brush), typeof(RangeBox),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty =
            Border.CornerRadiusProperty.AddOwner(typeof(RangeBox), new FrameworkPropertyMetadata(new CornerRadius()));
        public NumericType NumericType
        {
            get => (NumericType)GetValue(NumericTypeProperty);
            set => SetValue(NumericTypeProperty, value);
        }
        public static readonly DependencyProperty NumericTypeProperty =
            NumericBox.NumericTypeProperty.AddOwner(typeof(RangeBox),
                new FrameworkPropertyMetadata(NumericType.Float, null, new CoerceValueCallback(CoerceNumericType)));

        private static object CoerceNumericType(DependencyObject d, object baseValue)
        {
            RangeBox control = (RangeBox)d;
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

        /// <summary>
        /// 保留小数位数，最大15，-1表示不进行四舍五入
        /// </summary>
        public int Decimals
        {
            get => (int)GetValue(DecimalsProperty);
            set => SetValue(DecimalsProperty, value);
        }
        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(int), typeof(RangeBox),
                new FrameworkPropertyMetadata(3, new PropertyChangedCallback(OnDecimalsChanged), new CoerceValueCallback(CoerceDecimals)));
        private static void OnDecimalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBox slider = (RangeBox)d;
            slider.UpdateText();
        }
        private static object CoerceDecimals(DependencyObject d, object baseValue)
        {
            int value = (int)baseValue;
            if (value > MaxDecimals)
            {
                value = MaxDecimals;
            }

            return value;
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            internal set => SetValue(TextProperty, value);
        }
        internal static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RangeBox), new PropertyMetadata(string.Empty));

        public bool IsEditing
        {
            get => (bool)GetValue(IsEditingProperty);
            set => SetValue(IsEditingProperty, value);
        }
        public static readonly DependencyProperty IsEditingProperty =
            DependencyProperty.Register("IsEditing", typeof(bool), typeof(RangeBox), new FrameworkPropertyMetadata(false));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(RangeBox), new PropertyMetadata(false));

        private void UpdateText()
        {
            if (Decimals > -1)
            {
                Text = string.Format($"{{0:F{Decimals}}}", Value);
            }
            else
            {
                Text = Value.ToString();
            }
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            if (Decimals > -1)
            {
                double v1 = Math.Round(newValue, Decimals);
                if (v1 != Value)
                {
                    Value = v1;
                    return;
                }
            }
            base.OnValueChanged(oldValue, newValue);
            UpdateThumbWidth();
            UpdateText();
        }

        protected override void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
            base.OnMaximumChanged(oldMaximum, newMaximum);
            UpdateThumbWidth();
        }

        protected override void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
            base.OnMinimumChanged(oldMinimum, newMinimum);
            UpdateThumbWidth();
            NumericType = (NumericType)CoerceNumericType(this, NumericType);
        }

        private void RangeBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbWidth();
        }
        private void UpdateThumbWidth()
        {
            if (thumb == null)
            {
                return;
            }
            FrameworkElement parent = (FrameworkElement)thumb.Parent;
            double thumbWidth = parent.ActualWidth * (Value - Minimum) / (Maximum - Minimum);
            thumb.Width = thumbWidth;
            CornerRadius cornerRadius = new(CornerRadius.TopLeft, 0, 0, CornerRadius.BottomLeft);
            if (thumbWidth > parent.ActualWidth - CornerRadius.TopRight * 2)
            {
                cornerRadius.TopRight = CornerRadius.TopRight;
            }
            if (thumbWidth > parent.ActualWidth - CornerRadius.BottomRight * 2)
            {
                cornerRadius.BottomRight = CornerRadius.BottomRight;
            }
            thumb.CornerRadius = cornerRadius;
        }

        public override void OnApplyTemplate()
        {
            if (textBox != null)
            {
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.LostFocus -= TextBox_LostFocus;
            }
            SizeChanged -= RangeBox_SizeChanged;
            thumb = (Border)GetTemplateChild("PART_Thumb");
            textBox = (TextBox)GetTemplateChild("PART_TextBox");

            base.OnApplyTemplate();

            textBox.GotFocus += TextBox_GotFocus;
            textBox.LostFocus += TextBox_LostFocus;
            SizeChanged += RangeBox_SizeChanged;

            UpdateText();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            IsEditing = false;
        }


        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            IsEditing = true;
            if (!textBox.IsKeyboardFocusWithin)
            {
                _ = textBox.Focus();
            }
        }

        #region Mouse actions
        private Point startPoint;
        private double startValue;
        private bool dragging;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            base.OnMouseLeftButtonDown(e);

            //if (IsKeyboardFocused)
            //{
            //    OnMouseDoubleClick(null!);
            //    return;
            //}

            if (Keyboard.FocusedElement != this && Keyboard.FocusedElement != textBox)
            {
                Keyboard.Focus(this);
            }

            if (!CaptureMouse())
            {
                return;
            }

            startPoint = e.GetPosition(this);
            startValue = Value;
            dragging = true;
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            base.OnMouseMove(e);
            if (!dragging)
            {
                return;
            }

            UpdateValueWhenDrag(e.GetPosition(this));
            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                return;
            }

            try
            {
                base.OnMouseLeftButtonUp(e);
                if (dragging)
                {
                    UpdateValueWhenDrag(e.GetPosition(this));
                }
                e.Handled = true;
            }
            finally
            {
                dragging = false;
                ReleaseMouseCapture();
            }
        }

        private void UpdateValueWhenDrag(Point endPoint)
        {
            if (thumb == null)
            {
                return;
            }

            FrameworkElement parent = (FrameworkElement)thumb.Parent;
            double xOffset = endPoint.X - startPoint.X;
            double totalWidth = parent.ActualWidth;
            double scale = totalWidth / (Maximum - Minimum);
            double valOffset = xOffset / scale;
            if (Decimals > -1)
            {
                if (Math.Abs(valOffset) < Math.Pow(0.1, Decimals))
                {
                    return;
                }

                Value = Math.Clamp(Math.Round(startValue + valOffset, Decimals), Minimum, Maximum);
                return;
            }
            Value = Math.Clamp(startValue + valOffset, Minimum, Maximum);
        }
        #endregion

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsEditing)
            {
                return;
            }

            e.Handled = true;
            switch (e.Key)
            {
                case Key.Left:
                    Value -= SmallChange;
                    break;
                case Key.Right:
                    Value += SmallChange;
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
