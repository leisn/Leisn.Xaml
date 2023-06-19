// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Leisn.Xaml.Wpf.Extensions;

using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    public class PickColorDialog : Window
    {
        public Color SelectedColor { get; private set; }
        public double MagnifierScale { get; set; } = 4;
        public double MagnifierSize { get; set; } = 100;


        private SKBitmap _screenshot = null!;
        private readonly SKElement _contanier;
        public PickColorDialog()
        {
            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.None;
            ShowInTaskbar = false;
            Margin = Padding = new Thickness(0);
            Content = _contanier = new SKElement() { IgnorePixelScaling = true };
            _contanier.PaintSurface += PaintSurface;
            if (ImageEx.TryCaptureScreen(out SKBitmap? image))
                _screenshot = image;
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
                return;
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            canvas.DrawBitmap(_screenshot, 0, 0);
            canvas.SaveLayer();
            canvas.Save();
            var position = Mouse.GetPosition(_contanier).ToSKPoint();
            var radius = (float)MagnifierSize / 2;
            var magnifierImageLength = MagnifierSize / MagnifierScale;


            canvas.Restore();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var point = e.GetPosition(_contanier);
            SelectedColor = _screenshot.GetPixel((int)point.X, (int)point.Y).ToColor();
            DialogResult = true;
        }

    }
}
