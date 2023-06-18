// @Leisn (https://leisn.com , https://github.com/leisn)

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Leisn.Common.Media;

namespace Leisn.Xaml.Wpf.Controls
{
    public delegate void SelectedColorChangedEventHandler(object sender, SelectedColorChangedEventArgs e);
    public class SelectedColorChangedEventArgs : RoutedEventArgs
    {
        public Color OldValue { get; }
        public Color NewValue { get; }
        public SelectedColorChangedEventArgs(Color oldValue, Color newValue)
        {
            RoutedEvent = ColorPicker.SelectedColorChangedEvent;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    [TemplatePart(Name = PART_ColorSpectrumName, Type = typeof(ColorSpectrum))]
    public sealed class ColorPicker : Control
    {
        const string PART_ColorSpectrumName = "PART_ColorSpectrum";
        const string PART_TextBoxName = "PART_TextBox";
        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedColorChanged)));

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
           nameof(SelectedColorChanged), RoutingStrategy.Bubble, typeof(SelectedColorChangedEventHandler), typeof(ColorPicker));
        /// <summary>
        ///     An event fired when the color selection changes.
        /// </summary>
        [Category("Behavior")]
        public event SelectedColorChangedEventHandler SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }
            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }
        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cs = (ColorPicker)d;
            var value = (Color)e.NewValue;
            cs.RaiseEvent(new SelectedColorChangedEventArgs((Color)e.OldValue, value) { Source = cs });
            cs.UpdateToValues();
        }

        public int Alpha
        {
            get { return (int)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty, value); }
        }
        public static readonly DependencyProperty AlphaProperty =
            DependencyProperty.Register("Alpha", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        public int Red
        {
            get { return (int)GetValue(RedProperty); }
            set { SetValue(RedProperty, value); }
        }
        public static readonly DependencyProperty RedProperty =
            DependencyProperty.Register("Red", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        public int Green
        {
            get { return (int)GetValue(GreenProperty); }
            set { SetValue(GreenProperty, value); }
        }
        public static readonly DependencyProperty GreenProperty =
            DependencyProperty.Register("Green", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        public int Blue
        {
            get { return (int)GetValue(BlueProperty); }
            set { SetValue(BlueProperty, value); }
        }
        public static readonly DependencyProperty BlueProperty =
            DependencyProperty.Register("Blue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        private static void OnRgbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cs = (ColorPicker)d;
            cs.UpdateColorFromValues(false);
        }

        public int Hue
        {
            get { return (int)GetValue(HueProperty); }
            set { SetValue(HueProperty, value); }
        }
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0, new PropertyChangedCallback(OnHsvChanged)));

        public double Saturation
        {
            get { return (double)GetValue(SaturationProperty); }
            set { SetValue(SaturationProperty, value); }
        }
        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(ColorPicker), new PropertyMetadata(0d, new PropertyChangedCallback(OnHsvChanged)));

        public double Brightness
        {
            get { return (double)GetValue(BrightnessProperty); }
            set { SetValue(BrightnessProperty, value); }
        }
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(ColorPicker), new PropertyMetadata(1d, new PropertyChangedCallback(OnHsvChanged)));

        private static void OnHsvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cs = (ColorPicker)d;
            cs.UpdateColorFromValues(true);
        }

        private bool _pausePropertyChangedHandle;
        private ColorSpectrum _colorSpectrum = null!;
        private TextBox? _textBox;

        [MemberNotNull(nameof(_colorSpectrum))]
        public override void OnApplyTemplate()
        {
            if (_textBox != null)
            {
                _textBox.PreviewTextInput -= OnTextBoxInputing;
                _textBox.KeyDown -= OnTextBoxKeyDown;
            }
            if (_colorSpectrum != null)
            {
                _colorSpectrum.SelectedHsvChanged -= OnSpectrumHsvChagned;
            }
            base.OnApplyTemplate();

            _colorSpectrum = (ColorSpectrum)GetTemplateChild(PART_ColorSpectrumName);
            _colorSpectrum.SelectedHsvChanged += OnSpectrumHsvChagned;
            _colorSpectrum.SelectedRgb = SelectedColor.ToRgb();

            _textBox = GetTemplateChild(PART_TextBoxName) as TextBox;
            if (_textBox != null)
            {
                _textBox.KeyDown += OnTextBoxKeyDown;
                _textBox.PreviewTextInput += OnTextBoxInputing;
            }
        }

        #region 输入限制
        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Keyboard.FocusedElement is UIElement element)
                {
                    element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
        }

        private void OnTextBoxInputing(object sender, TextCompositionEventArgs e)
        {
            var textBox = (TextBox)sender;
            string regex = string.Format(@"^[a-fA-F0-9]+$");
            var match = Regex.IsMatch(e.Text, regex);
            if (!match)
            {
                e.Handled = true;
                FrameworkTextComposition? composition = e.TextComposition as FrameworkTextComposition;
                if (composition?.ResultLength > 0)//触发事件时，已完成输入
                {
                    textBox.Text = textBox.Text.Remove(composition.ResultOffset, composition.ResultLength);
                }
            }
        }
        #endregion


        private void OnSpectrumHsvChagned(object sender, SelectedHsvChangedEventArgs e)
        {
            if (_pausePropertyChangedHandle)
                return;
            _pausePropertyChangedHandle = true;
            try
            {
                var hsv = e.NewValue;
                if (hsv.H == Hue && hsv.S == Saturation && hsv.V == Brightness)
                    return;

                Hue = hsv.H;
                Saturation = hsv.S;
                Brightness = hsv.V;

                var rgb = hsv.ToRgb();
                Red = rgb.R;
                Green = rgb.G;
                Blue = rgb.B;
                SelectedColor = Color.FromArgb(SelectedColor.A, rgb.R, rgb.G, rgb.B);
            }
            finally
            {
                _pausePropertyChangedHandle = false;
            }
        }

        private void UpdateColorFromValues(bool fromHsv)
        {
            if (_pausePropertyChangedHandle)
            {
                return;
            }
            _pausePropertyChangedHandle = true;
            try
            {
                if (fromHsv)
                {
                    var hsv = new Hsv((ushort)Hue, Saturation, Brightness);
                    var rgb = hsv.ToRgb();
                    Red = rgb.R;
                    Green = rgb.G;
                    Blue = rgb.B;
                    _colorSpectrum.SelectedHue = new Hsv(hsv.H, 1, 1);
                    _colorSpectrum.SelectedHsv = hsv;
                    SelectedColor = Color.FromArgb((byte)Alpha, rgb.R, rgb.G, rgb.B);
                }
                else
                {
                    SelectedColor = Color.FromArgb((byte)Alpha, (byte)Red, (byte)Green, (byte)Blue);
                    var hsv = SelectedColor.ToHsv();
                    Hue = hsv.H;
                    Saturation = hsv.S;
                    Brightness = hsv.V;
                    _colorSpectrum.SelectedHue = new Hsv(hsv.H, 1, 1);
                    _colorSpectrum.SelectedRgb = SelectedColor.ToRgb();
                }
            }
            finally
            {
                _pausePropertyChangedHandle = false;
            }
        }

        private void UpdateToValues()
        {
            if (_pausePropertyChangedHandle)
            {
                return;
            }
            _pausePropertyChangedHandle = true;
            try
            {
                Red = SelectedColor.R;
                Green = SelectedColor.G;
                Blue = SelectedColor.B;
                Alpha = SelectedColor.A;
                var rgb = SelectedColor.ToRgb();
                var hsv = rgb.ToHsv();
                Hue = hsv.H;
                Saturation = hsv.S;
                Brightness = hsv.V;
                _colorSpectrum.SelectedRgb = rgb;
            }
            finally
            {
                _pausePropertyChangedHandle = false;
            }
        }
    }
}
