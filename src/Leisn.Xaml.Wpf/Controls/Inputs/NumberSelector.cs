// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_CanvasName, Type = typeof(SKElement))]
    [TemplatePart(Name = PART_PreviousButtonName, Type = typeof(ButtonBase))]
    [TemplatePart(Name = PART_NextButtonName, Type = typeof(ButtonBase))]
    internal class NumberSelector : Control
    {
        private const string PART_CanvasName = "PART_Canvas";
        private const string PART_PreviousButtonName = "PART_PreviousButton";
        private const string PART_NextButtonName = "PART_NextButton";

        private SKElement _canvas = null!;
        private ButtonBase? _previousButton;
        private ButtonBase? _nextButton;
        static NumberSelector()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(NumberSelector), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(NumberSelector), new FrameworkPropertyMetadata(true));
        }

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        public static readonly DependencyProperty CornerRadiusProperty = Border.CornerRadiusProperty.AddOwner(typeof(NumberSelector));


        public Thickness ItemPadding
        {
            get => (Thickness)GetValue(ItemPaddingProperty);
            set => SetValue(ItemPaddingProperty, value);
        }
        public static readonly DependencyProperty ItemPaddingProperty =
            DependencyProperty.Register("ItemPadding", typeof(Thickness), typeof(NumberSelector),
                 new FrameworkPropertyMetadata(new Thickness(), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }
        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(NumberSelector),
                  new FrameworkPropertyMetadata(TextAlignment.Center, new PropertyChangedCallback(OnPropertyChanged)));

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
        public static readonly DependencyProperty TextColorProperty =
            DependencyProperty.Register("TextColor", typeof(Color), typeof(NumberSelector),
                  new FrameworkPropertyMetadata(Colors.Bisque, new PropertyChangedCallback(OnPropertyChanged)));

        public Color SelectedBackground
        {
            get => (Color)GetValue(SelectedBackgroundProperty);
            set => SetValue(SelectedBackgroundProperty, value);
        }
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Color), typeof(NumberSelector),
                 new FrameworkPropertyMetadata(Colors.CadetBlue, new PropertyChangedCallback(OnPropertyChanged)));

        public Color SelectedTextColor
        {
            get => (Color)GetValue(SelectedTextColorProperty);
            set => SetValue(SelectedTextColorProperty, value);
        }
        public static readonly DependencyProperty SelectedTextColorProperty =
            DependencyProperty.Register("SelectedTextColor", typeof(Color), typeof(NumberSelector),
                 new FrameworkPropertyMetadata(Colors.White, new PropertyChangedCallback(OnPropertyChanged)));


        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(int), typeof(NumberSelector),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged)));

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(NumberSelector),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnPropertyChanged), new CoerceValueCallback(CoerceMaxValue)));

        private static object CoerceMaxValue(DependencyObject d, object baseValue)
        {
            NumberSelector ns = (NumberSelector)d;
            if (baseValue is int v && v < ns.MinValue)
            {
                return ns.MinValue;
            }

            return baseValue;
        }

        public int CurrentValue
        {
            get => (int)GetValue(CurrentValueProperty);
            set => SetValue(CurrentValueProperty, value);
        }
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(int), typeof(NumberSelector),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnPropertyChanged), new CoerceValueCallback(CoerceCurrentValue)));

        private static object CoerceCurrentValue(DependencyObject d, object baseValue)
        {
            NumberSelector ns = (NumberSelector)d;
            if (baseValue is int v)
            {
                v = Math.Clamp(v, ns.MinValue, ns.MaxValue);
                return v;
            }
            return baseValue;
        }

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberSelector ns = (NumberSelector)d;
            if (e.Property == MinValueProperty)
            {
                ns.CoerceValue(MaxValueProperty);
                ns.CoerceValue(CurrentValueProperty);
            }
            else if (e.Property == MaxValueProperty)
            {
                ns.CoerceValue(CurrentValueProperty);
            }
            ns.UpdateVisual();
        }

        public void UpdateVisual()
        {
            _canvas?.InvalidateVisual();
        }

        public override void OnApplyTemplate()
        {
            if (_canvas != null)
            {
                _canvas.PaintSurface -= OnPaintSurface;
            }
            if (_previousButton != null)
            {
                _previousButton.Click -= PreviousButtonClicked;
            }
            if (_nextButton != null)
            {
                _nextButton.Click -= NextButtonClicked;
            }
            _canvas = (SKElement)GetTemplateChild(PART_CanvasName);
            _previousButton = GetTemplateChild(PART_PreviousButtonName) as ButtonBase;
            _nextButton = GetTemplateChild(PART_NextButtonName) as ButtonBase;
            _canvas.PaintSurface += OnPaintSurface;
            if (_previousButton != null)
            {
                _previousButton.Click += PreviousButtonClicked;
            }
            if (_nextButton != null)
            {
                _nextButton.Click += NextButtonClicked;
            }
        }

        private void NextButtonClicked(object sender, RoutedEventArgs e)
        {
            NextValue();
        }

        private void PreviousButtonClicked(object sender, RoutedEventArgs e)
        {
            PreviousValue();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double width = 0, height = 0;
            using SKPaint textPaint = CreateTextPaint();
            string minStr = MinValue.ToString();
            string maxStr = MaxValue.ToString();
            string str = minStr.Length > maxStr.Length ? minStr : maxStr;
            SKRect bounds = new();
            textPaint.MeasureText(str, ref bounds);
            for (int i = MinValue; i <= MaxValue; i++)
            {
                width = Math.Max(width, bounds.Width + ItemPadding.Left + ItemPadding.Right);
                height += bounds.Height + ItemPadding.Top + ItemPadding.Bottom;
            }
            return new Size(double.IsInfinity(availableSize.Width) ? width : availableSize.Width,
                            double.IsInfinity(availableSize.Height) ? height : availableSize.Height);
        }

        private SKPaint CreateTextPaint()
        {
            string familyName = FontFamily.FamilyNames[XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)];
            SKFontStyleWeight weight = (SKFontStyleWeight)FontWeight.ToOpenTypeWeight();
            SKFontStyleSlant style = SKFontStyleSlant.Upright;
            if (FontStyle == FontStyles.Italic)
            {
                style = SKFontStyleSlant.Italic;
            }
            else if (FontStyle == FontStyles.Oblique)
            {
                style = SKFontStyleSlant.Oblique;
            }

            SKFontStyleWidth width = SKFontStyleWidth.Normal;
            if (FontStretch == FontStretches.Condensed)
            {
                width = SKFontStyleWidth.Condensed;
            }
            else if (FontStretch == FontStretches.Expanded)
            {
                width = SKFontStyleWidth.Expanded;
            }
            else if (FontStretch == FontStretches.ExtraExpanded)
            {
                width = SKFontStyleWidth.ExtraExpanded;
            }
            else if (FontStretch == FontStretches.ExtraCondensed)
            {
                width = SKFontStyleWidth.ExtraCondensed;
            }
            else if (FontStretch == FontStretches.SemiExpanded)
            {
                width = SKFontStyleWidth.SemiExpanded;
            }
            else if (FontStretch == FontStretches.SemiCondensed)
            {
                width = SKFontStyleWidth.SemiCondensed;
            }
            else if (FontStretch == FontStretches.UltraExpanded)
            {
                width = SKFontStyleWidth.UltraExpanded;
            }
            else if (FontStretch == FontStretches.ExtraCondensed)
            {
                width = SKFontStyleWidth.UltraCondensed;
            }

            return new SKPaint
            {
                TextSize = (float)FontSize,
                Typeface = SKTypeface.FromFamilyName(familyName, weight, width, style),
                IsAntialias = true,
                TextEncoding = SKTextEncoding.Utf8,
            };
        }

        //private readonly Dictionary<Rect, int> _numberAreas = new Dictionary<Rect, int>();

        protected virtual void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            //_numberAreas.Clear();
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();

            using SKPaint paint = CreateTextPaint();
            SKRect textBounds = new();
            paint.MeasureText(MaxValue.ToString(), ref textBounds);

            using SKPaint selectPaint = new() { IsStroke = false, Color = SelectedBackground.ToSKColor() };
            canvas.DrawRect(0, e.Info.Rect.MidY - textBounds.Height / 2 - (float)ItemPadding.Top,
                e.Info.Rect.Width, textBounds.Height + (float)(ItemPadding.Top + ItemPadding.Bottom), selectPaint);

            float top = e.Info.Rect.MidY - textBounds.Top - textBounds.Height / 2;
            float left = (float)ItemPadding.Left;

            float space = (float)(ItemPadding.Bottom + ItemPadding.Top) + textBounds.Height;
            paint.Color = SelectedTextColor.ToSKColor();
            DrawText(CurrentValue);
            top += space;
            paint.Color = TextColor.ToSKColor();
            int index = CurrentValue - MinValue + 1;
            while (top < e.Info.Rect.Bottom)
            {
                int value = GetValue(index);
                DrawText(value);
                top += space;
                index++;
            }
            top = e.Info.Rect.MidY - textBounds.Top - textBounds.Height / 2 - space;
            index = CurrentValue - MinValue - 1;
            while (top > ItemPadding.Top)
            {
                int value = GetValue(index);
                DrawText(value);
                top -= space;
                index--;
            }

            void DrawText(int value)
            {
                string text = value.ToString();
                float width = paint!.MeasureText(text);
                left = TextAlignment switch
                {
                    TextAlignment.Right => e.Info.Rect.Right - width - (float)ItemPadding.Right,
                    TextAlignment.Center => e.Info.Rect.MidX - width / 2,
                    _ => (float)ItemPadding.Left,
                };
                canvas.DrawText(text, left, top, paint);
                //_numberAreas.Add(new Rect(0, top - ItemPadding.Top, e.Info.Rect.Width, textBounds.Height + ItemPadding.Top + ItemPadding.Bottom), value);
            }
        }


        private int GetValue(int index)
        {
            if (index > 0)
            {
                return index % (MaxValue - MinValue + 1) + MinValue;
            }
            else if (index < 0)
            {
                return (index + 1) % (MaxValue - MinValue + 1) + MaxValue;
            }
            else
            {
                return MinValue;
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                PreviousValue();
            }
            else
            {
                NextValue();
            }
            e.Handled = true;
        }

        public void NextValue()
        {
            CurrentValue = GetValue(CurrentValue - MinValue + 1);
        }

        public void PreviousValue()
        {
            CurrentValue = GetValue(CurrentValue - MinValue - 1);
        }

    }
}
