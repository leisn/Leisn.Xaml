// By Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls.Inputs
{
    [TemplatePart(Name = "PART_Thumb", Type = typeof(Border))]
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    public class RangeBox : RangeBase
    {
        private Border thumb = null!;
        private TextBox textBox = null!;

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
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(RangeBox), new FrameworkPropertyMetadata(new CornerRadius()));

        public int Decimals
        {
            get => (int)GetValue(DecimalsProperty);
            set => SetValue(DecimalsProperty, value);
        }
        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register("Decimals", typeof(int), typeof(RangeBox), new PropertyMetadata(3, new PropertyChangedCallback(OnDecimalsChanged)));

        private static void OnDecimalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RangeBox slider = (RangeBox)d;
            slider.UpdateText();
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            internal set => SetValue(TextProperty, value);
        }
        internal static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RangeBox), new PropertyMetadata("0.000"));

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


        static RangeBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RangeBox), new FrameworkPropertyMetadata(typeof(RangeBox)));
        }

        private void UpdateText()
        {
            Text = string.Format($"{{0:F{Decimals}}}", Value);
        }

        protected override void OnValueChanged(double oldValue, double newValue)
        {
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
            if (textBox is NumericBox box)
            {
                box.NumericType = newMinimum < 0 ? NumericType.Float : NumericType.UFloat;
            }
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
            if (thumbWidth > parent.ActualWidth - (CornerRadius.TopRight * 2))
            {
                cornerRadius.TopRight = CornerRadius.TopRight;
            }
            if (thumbWidth > parent.ActualWidth - (CornerRadius.BottomRight * 2))
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
            if (e.Source == this)
            {
                IInputElement focused = FocusManager.GetFocusedElement(FocusManager.GetFocusScope(this));
                if (focused != this && focused != textBox)
                {
                    FocusManager.SetFocusedElement(FocusManager.GetFocusScope(this), this);
                }
            }
            if (!CaptureMouse())
            {
                return;
            }

            startPoint = e.GetPosition(this);
            startValue = Value;
            dragging = true;
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
            if (Math.Abs(valOffset) < Math.Pow(0.1, Decimals))
            {
                return;
            }
            Value = Math.Round(startValue + valOffset, Decimals);
        }
        #endregion

        //protected override void OnKeyDown(KeyRoutedEventArgs e)
        //{
        //    base.OnKeyDown(e);
        //    switch (e.Key)
        //    {
        //        case VirtualKey.Left:
        //            Value -= 0.001;
        //            break;
        //        case VirtualKey.Right:
        //            Value += 0.001;
        //            break;
        //        case VirtualKey.Up:
        //            Value -= 0.01;
        //            break;
        //        case VirtualKey.Down:
        //            Value += 0.01;
        //            break;
        //        case VirtualKey.PageUp:
        //            Value -= 0.1;
        //            break;
        //        case VirtualKey.PageDown:
        //            Value += 0.1;
        //            break;
        //        case VirtualKey.Home:
        //            Value = Minimum;
        //            break;
        //        case VirtualKey.End:
        //            Value = Maximum;
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}
