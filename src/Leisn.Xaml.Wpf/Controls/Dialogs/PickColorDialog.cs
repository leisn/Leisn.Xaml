// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

using Leisn.Common.Media;
using Leisn.Xaml.Wpf.Extensions;

using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    public class PickColorDialog : Window
    {
        public Color SelectedColor { get; private set; }
        public float MagnifierScale { get; set; } = 4;
        public float MagnifierSize { get; set; } = 100;
        public float StrokeWidth { get; set; } = 4;
        public float TextLineSpacing { get; set; } = 8;

        private readonly SKBitmap _screenshot = null!;
        private readonly SKElement _contanier;
        private readonly SKColor _primaryColor;
        public PickColorDialog()
        {
            Topmost = true;
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Margin = Padding = new Thickness(0);
            _primaryColor = ((Color)FindResource("PrimaryColor")).ToSKColor();
            Content = _contanier = new SKElement() { IgnorePixelScaling = true };
            _contanier.PaintSurface += PaintSurface;
            if (ImageEx.TryCaptureScreen(out SKBitmap? image))
            {
                _screenshot = image;
            }

            Loaded += OnLoaded;
        }

        protected override void OnClosed(EventArgs e)
        {
            _screenshot?.Dispose();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (_screenshot == null)
            {
                DialogResult = false;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            _contanier.InvalidateVisual();
        }

        private void PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
        {
            if (_screenshot == null)
            {
                return;
            }

            float rightAreaPadding = StrokeWidth;
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();
            canvas.DrawBitmap(_screenshot, e.Info.Rect);

            if (!FontFamily.FamilyNames.TryGetValue(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag), out string? fontName))
            {
                fontName = FontFamily.FamilyNames.FirstOrDefault().Value;
            }
            using SKPaint paint = new()
            {
                Color = _primaryColor,
                IsAntialias = false,
                IsStroke = true,
                StrokeWidth = StrokeWidth,
                Typeface = fontName is null ? SKTypeface.Default : SKTypeface.FromFamilyName(fontName),
                TextSize = (float)FontSize,
            };
            string measureText = new Hsv(360, 1, 1).ToString();
            float textWidth = paint.MeasureText(measureText);

            float halfStroke = StrokeWidth / 2;
            canvas.DrawRect(((SKRect)e.Info.Rect).OffsetToCenter(halfStroke), paint);
            canvas.SaveLayer();
            canvas.Save();
            Point mousePosition = Mouse.GetPosition(_contanier);
            SKPoint position = mousePosition.ToSKPoint();
            float radius = MagnifierSize / 2;
            float imageOffset = MagnifierSize / MagnifierScale / 2;
            SKRect rect = new(
                Math.Max(0, position.X - imageOffset),
                Math.Max(0, position.Y - imageOffset),
                Math.Min(position.X + imageOffset, _screenshot.Width),
                Math.Min(position.Y + imageOffset, _screenshot.Height));


            float offsetH = radius + 40, offsetV = radius + 10;
            if (position.X + offsetH + radius > e.Info.Width)
            {
                offsetH = -offsetH;
            }

            if (position.Y + offsetV + radius > e.Info.Height)
            {
                offsetV = -offsetV;
            }

            position.X += offsetH;
            position.Y += offsetV;

            SKRect magnifierRect = SkiaEx.CreateRect(position, radius);
            SKRect contanierRect = new(magnifierRect.Left, magnifierRect.Top,
                magnifierRect.Right + Math.Max(radius * 2, textWidth + rightAreaPadding * 2), magnifierRect.Bottom);
            canvas.ClipRect(contanierRect, antialias: true);
            //background
            paint.IsStroke = false;
            paint.Color = SKColors.White;
            canvas.DrawRect(contanierRect, paint);

            contanierRect = contanierRect.OffsetToCenter(halfStroke);
            SKRect bitmapRect = magnifierRect.OffsetToCenter(halfStroke);
            //bitmap
            canvas.DrawBitmap(_screenshot, rect, bitmapRect);
            //border
            paint.StrokeWidth = halfStroke;
            paint.Color = _primaryColor;
            paint.IsStroke = true;
            canvas.DrawRect(contanierRect, paint);
            //+
            magnifierRect = bitmapRect.OffsetToCenter(magnifierRect.Width / 3);
            float centerY = magnifierRect.MidY - halfStroke / 2;
            canvas.DrawLine(magnifierRect.Left, centerY, magnifierRect.Right, centerY, paint);
            float centerX = magnifierRect.MidX - halfStroke / 2;
            canvas.DrawLine(centerX, magnifierRect.Top, centerX, magnifierRect.Bottom, paint);
            // color
            SKColor color = _screenshot.GetPixel((int)mousePosition.X, (int)mousePosition.Y);
            SKRect colorRect = SKRect.Create(bitmapRect.Right + StrokeWidth + rightAreaPadding, bitmapRect.Top + StrokeWidth, 24 + StrokeWidth, 24 + StrokeWidth);
            canvas.DrawRect(colorRect, paint);
            paint.IsStroke = false;
            paint.Color = color;
            canvas.DrawRect(colorRect, paint);
            //text
            paint.IsStroke = false;
            paint.IsAntialias = true;
            paint.IsAutohinted = true;
            paint.Color = SKColors.Black;
            Rgb rgb = new(color.Red, color.Green, color.Blue);
            colorRect.Bottom += TextLineSpacing + paint.TextSize;
            canvas.DrawText(rgb.ToString(), colorRect.Left, colorRect.Bottom, paint);
            colorRect.Bottom += TextLineSpacing + paint.TextSize;
            canvas.DrawText(rgb.ToHsv().ToString(), colorRect.Left, colorRect.Bottom, paint);

            canvas.Restore();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(_contanier);
            SelectedColor = _screenshot.GetPixel((int)position.X, (int)position.Y).ToColor();
            DialogResult = true;
        }

    }
}
