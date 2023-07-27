// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Leisn.Xaml.Wpf.Extensions;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    [TemplatePart(Name = PART_ColorPickerName, Type = typeof(ColorPicker))]
    [TemplatePart(Name = PART_PopupName, Type = typeof(Popup))]
    public class ColorPickerEditor : Control, IPropertyEditor
    {
        private const string PART_ColorPickerName = "PART_ColorPicker";
        private const string PART_PopupName = "PART_Popup";
        private ColorPicker _colorPicker = null!;
        private Popup _popup = null!;

        static ColorPickerEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerEditor), new FrameworkPropertyMetadata(typeof(ColorPickerEditor)));
            EventManager.RegisterClassHandler(typeof(ColorPickerEditor), Mouse.LostMouseCaptureEvent, new MouseEventHandler(OnLostMouseCapture));
            EventManager.RegisterClassHandler(typeof(ColorPickerEditor), Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButtonDown), true); // call us even if the transparent button in the style gets the click.
        }
        public FrameworkElement CreateElement(PropertyItem item)
        {
            IsReadOnly = item.IsReadOnly;
            return this;
        }

        public DependencyProperty GetBindingProperty()
        {
            return SelectedColorProperty;
        }

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPickerEditor),
                new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedColorChanged)));
        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorPickerEditor)d).UpdateColors();
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(ColorPickerEditor), new PropertyMetadata(false, new PropertyChangedCallback(OnIsReadOnlyChanged)));

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerEditor c = (ColorPickerEditor)d;
            if (c._colorPicker != null)
            {
                c._colorPicker.IsEnabled = !(bool)e.NewValue;
            }
        }

        public Color NoAlphaColor
        {
            get => (Color)GetValue(NoAlphaColorProperty);
            set => SetValue(NoAlphaColorProperty, value);
        }
        public static readonly DependencyProperty NoAlphaColorProperty =
            DependencyProperty.Register("NoAlphaColor", typeof(Color), typeof(ColorPickerEditor), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(ColorPickerEditor), new PropertyMetadata(false, new PropertyChangedCallback(OnIsDropDownOpenChanged)));

        private static void OnIsDropDownOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorPickerEditor picker = (ColorPickerEditor)d;
            picker.OnIsDropDownChanged();
        }

        public override void OnApplyTemplate()
        {
            if (_colorPicker is not null)
            {
                _colorPicker.SelectedColorChanged -= OnPickerColorChanged;
            }
            base.OnApplyTemplate();
            _popup = (Popup)GetTemplateChild(PART_PopupName);

            _colorPicker = (ColorPicker)GetTemplateChild(PART_ColorPickerName);
            _colorPicker.SelectedColor = SelectedColor;
            _colorPicker.SelectedColorChanged += OnPickerColorChanged; ;
            _colorPicker.IsEnabled = !IsReadOnly;
        }

        private void OnPickerColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            SelectedColor = e.NewValue;
        }

        private void UpdateColors()
        {
            NoAlphaColor = Color.FromRgb(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            if (_colorPicker is null)
            {
                return;
            }
            if (!Equals(SelectedColor, _colorPicker.SelectedColor))
            {
                _colorPicker.SelectedColor = SelectedColor;
            }
        }
        #region IsDropDownOpen Popup
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                IsDropDownOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                IsDropDownOpen = true;
            }
        }

        private void OnIsDropDownChanged()
        {
            if (IsDropDownOpen)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }
            else if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }
        }


        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            if (_popup.IsOpen && !IsKeyboardFocusWithin)
            {
                IsDropDownOpen = false;
            }
        }

        private static void OnMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorPickerEditor editor = (ColorPickerEditor)sender;

            if (!editor.IsKeyboardFocusWithin)
            {
                editor.Focus();
            }
            e.Handled = true;

            if (Mouse.Captured == editor && e.OriginalSource == editor)
            {
                editor.IsDropDownOpen = false;
            }
        }
        private static void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            ColorPickerEditor editor = (ColorPickerEditor)sender;
            IInputElement captured = Mouse.Captured;
            if (captured != editor)
            {
                if (e.OriginalSource == editor)
                {
                    if (captured == null || !editor.HasDescendant(captured as DependencyObject))
                    {
                        editor.IsDropDownOpen = false;
                    }
                }
                else
                {
                    if (editor.HasDescendant(e.OriginalSource as DependencyObject))
                    {
                        if (editor.IsDropDownOpen && captured == null)
                        {
                            Mouse.Capture(editor, CaptureMode.SubTree);
                            e.Handled = true;
                        }
                    }
                    else
                    {
                        editor.IsDropDownOpen = false;
                    }
                }
            }
        }
        #endregion
    }
}
