using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Leisn.Xaml.Wpf.Extensions;

using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    public class DateTimeClock : SKElement
    {
        static DateTimeClock()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(DateTimeClock), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(DateTimeClock), new FrameworkPropertyMetadata(true));
        }

        private DpiScale _dpiScale;
        public DateTimeClock()
        {
            IgnorePixelScaling = false;
            _dpiScale = VisualEx.GetDpiScale();
        }
        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            _dpiScale = newDpi;
            InvalidateVisual();
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();
            var size = Math.Min(e.Info.Width, e.Info.Height);
            canvas.Translate(e.Info.Rect.MidX, e.Info.Rect.MidY);
            float radius = size / 2f - 1;
            float spacing = 3f;
            var circleLength = (radius - 10 - spacing * 5) / 6;

            using var circlePaint = new SKPaint() { IsStroke = false, IsAntialias = true, Color = SKColors.Gray };
            for (int i = 0; i < 6; i++)
            {
                var outer = radius - (circleLength + spacing) * i;
                using (SKPath path = new() { FillType = SKPathFillType.EvenOdd })
                {
                    path.AddCircle(0, 0, outer);
                    path.AddCircle(0, 0, outer - circleLength);
                    canvas.DrawPath(path, circlePaint);
                }
            }

            base.OnPaintSurface(e);
        }

        private Point GetRealPoint(Point point)
        {
            return new(point.X * _dpiScale.DpiScaleX, point.Y * _dpiScale.DpiScaleY);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var point = GetRealPoint(e.GetPosition(this));
        }
    }
}
