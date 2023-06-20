// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls.Editors
{
    [TemplatePart(Name = PART_ColorPickerName, Type = typeof(ColorPicker))]
    [TemplatePart(Name = PART_PopupName, Type = typeof(Popup))]
    public class ColorPickerEditor : Control, IPropertyEditor
    {
        const string PART_ColorPickerName = "PART_ColorPicker";
        const string PART_PopupName = "PART_Popup";
        private ColorPicker _colorPicker = null!;
        private Popup _popup = null!;

        static ColorPickerEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerEditor), new FrameworkPropertyMetadata(typeof(ColorPickerEditor)));
        }

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPickerEditor),
                new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnSelectedColorChanged)));
        private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorPickerEditor)d).UpdateColors();
        }

        public Color NoAlphaColor
        {
            get { return (Color)GetValue(NoAlphaColorProperty); }
            set { SetValue(NoAlphaColorProperty, value); }
        }
        public static readonly DependencyProperty NoAlphaColorProperty =
            DependencyProperty.Register("NoAlphaColor", typeof(Color), typeof(ColorPickerEditor), new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsRender));


        public FrameworkElement CreateElement(PropertyItem item) => this;
        public DependencyProperty GetBindingProperty() => SelectedColorProperty;

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
            _colorPicker.SelectedColorChanged += OnPickerColorChanged;
        }

        private void OnPickerColorChanged(object sender, SelectedColorChangedEventArgs e)
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

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);
            if (_popup.IsOpen && !IsKeyboardFocusWithin)
                _popup.IsOpen = false;
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _popup.IsOpen = false;
            }
            else if (e.Key == Key.Enter)
            {
                _popup.IsOpen = true;
            }
        }
    }
}
