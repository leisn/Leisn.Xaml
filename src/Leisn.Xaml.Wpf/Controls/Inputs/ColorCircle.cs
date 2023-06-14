// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    /// <summary>
    /// 色相环
    /// </summary>
    public class ColorCircle : SKElement
    {
        static ColorCircle()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(ColorCircle), new FrameworkPropertyMetadata(true));
        }

        public double CircleLength
        {
            get { return (double)GetValue(CircleLengthProperty); }
            set { SetValue(CircleLengthProperty, value); }
        }
        public static readonly DependencyProperty CircleLengthProperty =
            DependencyProperty.Register("CircleLength", typeof(double), typeof(ColorCircle),
                new FrameworkPropertyMetadata(.3d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(ColorCircle),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Color), typeof(ColorCircle),
                new FrameworkPropertyMetadata(Colors.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));


        static readonly SKColor[] _colors = new SKColor[]
        {
            new SKColor(255, 0, 0),
            new SKColor(255, 255, 0),
            new SKColor(0, 255, 0),
            new SKColor(0, 255, 255),
            new SKColor(0, 0, 255),
            new SKColor(255, 0, 255),
            new SKColor(255, 0, 0)
        };
        static readonly float[] _colorPos = new float[] { 0, 0.167f, 0.334f, 0.501f, 0.668f, 0.835f, 1 };

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            var center = new SKPoint(e.Info.Rect.MidX, e.Info.Rect.MidY);
            float length = Math.Min(e.Info.Width, e.Info.Height) - 10;
            float outerRadius = length / 2 - (float)BorderWidth;
            float circleLength = (float)(CircleLength > 1 ? CircleLength : CircleLength * outerRadius);
            float innerRadius = outerRadius - circleLength;
            canvas.Save();
            canvas.Translate(center);
            canvas.RotateDegrees(-90);
            using var paint = new SKPaint
            {
                IsStroke = false,
                IsAntialias = true,
            };
            //draw colors
            #region draw
            using (var shade = SKShader.CreateSweepGradient(new SKPoint(), _colors, _colorPos))
            using (var clip = new SKPath { FillType = SKPathFillType.EvenOdd })
            {
                clip.AddCircle(0, 0, innerRadius);
                clip.AddCircle(0, 0, outerRadius);
                canvas.ClipPath(clip, SKClipOperation.Intersect, true);
                paint.Shader = shade;
                canvas.DrawPaint(paint);
            }

            #endregion

            //draw broder
            if (BorderWidth > 0)
            {
                paint.Color = BorderColor.ToSKColor();
                paint.StrokeWidth = (float)BorderWidth;
                paint.IsStroke = true;

                canvas.DrawCircle(0, 0, innerRadius, paint);
                canvas.DrawCircle(0, 0, outerRadius, paint);
            }
            canvas.Restore();
            base.OnPaintSurface(e);
        }
    }
}
