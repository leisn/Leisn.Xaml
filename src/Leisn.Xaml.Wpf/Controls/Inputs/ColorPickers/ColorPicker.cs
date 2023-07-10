// @Leisn (https://leisn.com , https://github.com/leisn)

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Leisn.Common.Media;
using Leisn.Xaml.Wpf.Internals;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_ColorSpectrumName, Type = typeof(ColorSpectrum))]
    [TemplatePart(Name = PART_TextBoxName, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_PickScreenButtonName, Type = typeof(ButtonBase))]
    public sealed class ColorPicker : Control
    {
        private const string PART_ColorSpectrumName = "PART_ColorSpectrum";
        private const string PART_TextBoxName = "PART_TextBox";
        private const string PART_PickScreenButtonName = "PART_PickScreenButton";

        private ColorSpectrum _colorSpectrum = null!;
        private TextBox? _textBox;
        private ButtonBase? _pickScreenButton;

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedColorChanged)));

        public static readonly RoutedEvent SelectedColorChangedEvent =
          EventManager.RegisterRoutedEvent(nameof(SelectedColorChanged), RoutingStrategy.Bubble,
              typeof(RoutedPropertyChangedEventHandler<Color>), typeof(ColorPicker));
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add => AddHandler(SelectedColorChangedEvent, value);
            remove => RemoveHandler(SelectedColorChangedEvent, value);
        }

        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker cs = (ColorPicker)d;
            Color value = (Color)e.NewValue;
            cs.RaiseEvent(new RoutedPropertyChangedEventArgs<Color>((Color)e.OldValue, value, SelectedColorChangedEvent));
            cs.UpdateToValues();
        }

        public ColorSpectrumStyle SpectrumStyle
        {
            get => (ColorSpectrumStyle)GetValue(SpectrumStyleProperty);
            set => SetValue(SpectrumStyleProperty, value);
        }
        public static readonly DependencyProperty SpectrumStyleProperty = ColorSpectrum.SpectrumStyleProperty.AddOwner(typeof(ColorPicker));

        public int Alpha
        {
            get => (int)GetValue(AlphaProperty);
            set => SetValue(AlphaProperty, value);
        }
        public static readonly DependencyProperty AlphaProperty =
            DependencyProperty.Register("Alpha", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        public int Red
        {
            get => (int)GetValue(RedProperty);
            set => SetValue(RedProperty, value);
        }
        public static readonly DependencyProperty RedProperty =
            DependencyProperty.Register("Red", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        public int Green
        {
            get => (int)GetValue(GreenProperty);
            set => SetValue(GreenProperty, value);
        }
        public static readonly DependencyProperty GreenProperty =
            DependencyProperty.Register("Green", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        public int Blue
        {
            get => (int)GetValue(BlueProperty);
            set => SetValue(BlueProperty, value);
        }
        public static readonly DependencyProperty BlueProperty =
            DependencyProperty.Register("Blue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0xFF, new PropertyChangedCallback(OnRgbChanged)));

        private static void OnRgbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker cs = (ColorPicker)d;
            cs.UpdateColorFromValues(false);
        }

        public int Hue
        {
            get => (int)GetValue(HueProperty);
            set => SetValue(HueProperty, value);
        }
        public static readonly DependencyProperty HueProperty =
            DependencyProperty.Register("Hue", typeof(int), typeof(ColorPicker), new PropertyMetadata(0, new PropertyChangedCallback(OnHsvChanged)));

        public double Saturation
        {
            get => (double)GetValue(SaturationProperty);
            set => SetValue(SaturationProperty, value);
        }
        public static readonly DependencyProperty SaturationProperty =
            DependencyProperty.Register("Saturation", typeof(double), typeof(ColorPicker), new PropertyMetadata(0d, new PropertyChangedCallback(OnHsvChanged)));

        public double Brightness
        {
            get => (double)GetValue(BrightnessProperty);
            set => SetValue(BrightnessProperty, value);
        }
        public static readonly DependencyProperty BrightnessProperty =
            DependencyProperty.Register("Brightness", typeof(double), typeof(ColorPicker), new PropertyMetadata(1d, new PropertyChangedCallback(OnHsvChanged)));

        private static void OnHsvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPicker cs = (ColorPicker)d;
            cs.UpdateColorFromValues(true);
        }

        [MemberNotNull(nameof(_colorSpectrum))]
        public override void OnApplyTemplate()
        {
            if (_pickScreenButton != null)
            {
                _pickScreenButton.Click -= OnPickScreenColorClicked;
            }
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

            _pickScreenButton = GetTemplateChild(PART_PickScreenButtonName) as ButtonBase;
            if (_pickScreenButton != null)
            {
                _pickScreenButton.Click += OnPickScreenColorClicked;
            }
        }

        private void OnPickScreenColorClicked(object sender, RoutedEventArgs e)
        {
            PickColorDialog dialog = new() { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            SelectedColor = dialog.SelectedColor;
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
            TextBox textBox = (TextBox)sender;
            string regex = string.Format(@"^[a-fA-F0-9]+$");
            bool match = Regex.IsMatch(e.Text, regex);
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

        private void OnSpectrumHsvChagned(object sender, RoutedPropertyChangedEventArgs<Hsv> e)
        {
            try
            {
                if (!EditorUtil.BeginEdit(this))
                {
                    return;
                }

                ColorSpectrum spectrum = (ColorSpectrum)sender;
                Hsv hsv = spectrum.SelectedHsv;
                if (hsv.H == Hue && hsv.S == Saturation && hsv.V == Brightness)
                {
                    return;
                }
                Hue = hsv.H;
                Saturation = hsv.S;
                Brightness = hsv.V;

                Rgb rgb = spectrum.SelectedRgb;
                if (rgb.R == Red && rgb.G == Green && rgb.B == Blue)
                {
                    return;
                }
                Red = rgb.R;
                Green = rgb.G;
                Blue = rgb.B;
                SelectedColor = Color.FromArgb(SelectedColor.A, rgb.R, rgb.G, rgb.B);
            }
            finally
            {
                EditorUtil.EndEdit(this);
            }
        }

        private void UpdateColorFromValues(bool fromHsv)
        {
            try
            {
                if (!EditorUtil.BeginEdit(this))
                {
                    return;
                }

                if (_colorSpectrum == null)
                {
                    return;
                }
                if (fromHsv)
                {
                    Hsv hsv = new((ushort)Hue, Saturation, Brightness);
                    Rgb rgb = hsv.ToRgb();
                    Red = rgb.R;
                    Green = rgb.G;
                    Blue = rgb.B;
                    SelectedColor = Color.FromArgb((byte)Alpha, rgb.R, rgb.G, rgb.B);
                    _colorSpectrum.SelectedHsv = hsv;
                }
                else
                {
                    SelectedColor = Color.FromArgb((byte)Alpha, (byte)Red, (byte)Green, (byte)Blue);
                    Hsv hsv = SelectedColor.ToHsv();
                    Hue = hsv.H;
                    Saturation = hsv.S;
                    Brightness = hsv.V;
                    _colorSpectrum.SelectedRgb = SelectedColor.ToRgb();
                }
            }
            finally
            {
                EditorUtil.EndEdit(this);
            }
        }

        private void UpdateToValues()
        {
            try
            {
                if (!EditorUtil.BeginEdit(this))
                {
                    return;
                }

                Alpha = SelectedColor.A;
                Red = SelectedColor.R;
                Green = SelectedColor.G;
                Blue = SelectedColor.B;
                Rgb rgb = SelectedColor.ToRgb();
                Hsv hsv = rgb.ToHsv();
                Hue = hsv.H;
                Saturation = hsv.S;
                Brightness = hsv.V;
                if (_colorSpectrum is not null)
                {
                    _colorSpectrum.SelectedRgb = rgb;
                }
            }
            finally
            {
                EditorUtil.EndEdit(this);
            }
        }

    }
}
