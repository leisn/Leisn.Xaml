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
            new SKColor(255, 0, 255),
            new SKColor(0, 0, 255),
            new SKColor(0, 255, 255),
            new SKColor(0, 255, 0),
            new SKColor(255, 255, 0),
            new SKColor(255, 0, 0),
        };
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

            using var paint = new SKPaint
            {
                IsStroke = false,
                IsAntialias = true,
            };
            //draw colors
            #region draw 1
            float areaAngle = 360f / (_colors.Length - 1) / 256;
            float areaRadians = (float)(Math.PI * 2 / (_colors.Length - 1) / 256);
            var areaRotation = Matrix3x2.CreateRotation(areaRadians);
            Vector2 point1 = Vector2.Transform(new Vector2(innerRadius, 0), areaRotation);
            Vector2 point2 = Vector2.Transform(new Vector2(outerRadius, 0), areaRotation);
            SKColor color;
            //for (int i = 0; i < _colors.Length - 1; i++)
            //{
            //    color = _colors[i];
            //    byte j = 0;
            //    while (true)
            //    {
            //        var val = i % 2 == 0 ? j : (byte)(255 - j);
            //        switch (i)
            //        {
            //            case 0:
            //                color = color.WithBlue(val);
            //                break;
            //            case 1:
            //                color = color.WithRed(val);
            //                break;
            //            case 2:
            //                color = color.WithGreen(val);
            //                break;
            //            case 3:
            //                color = color.WithBlue(val);
            //                break;
            //            case 4:
            //                color = color.WithRed(j);
            //                break;
            //            case 5:
            //                color = color.WithGreen(val);
            //                break;
            //        }
            //        paint.Color = color;

            //        using (var path = new SKPath())
            //        {
            //            path.MoveTo(innerRadius, 0);
            //            path.ArcTo(innerRadius, innerRadius, areaAngle, SKPathArcSize.Small, SKPathDirection.Clockwise, point1.X, point1.Y);
            //            path.LineTo(point2.X, point2.Y);
            //            path.ArcTo(outerRadius, outerRadius, areaAngle, SKPathArcSize.Small, SKPathDirection.CounterClockwise, outerRadius, 0);
            //            path.LineTo(innerRadius, 0);
            //            path.Close();
            //            canvas.DrawPath(path, paint);
            //        }
            //        canvas.RotateDegrees(areaAngle, 0, 0);
            //        if (j == 255)
            //            break;
            //        j++;
            //    }
            //}
            #endregion

            #region draw2
            for (int i = 0; i < _colors.Length - 1; i++)
            {
                color = _colors[i];
                byte j = 0;
                while (true)
                {
                    var val = i % 2 == 0 ? j : (byte)(255 - j);
                    switch (i)
                    {
                        case 0:
                            color = color.WithBlue(val);
                            break;
                        case 1:
                            color = color.WithRed(val);
                            break;
                        case 2:
                            color = color.WithGreen(val);
                            break;
                        case 3:
                            color = color.WithBlue(val);
                            break;
                        case 4:
                            color = color.WithRed(j);
                            break;
                        case 5:
                            color = color.WithGreen(val);
                            break;
                    }
                    paint.Color = color;

                    using (var path = new SKPath())
                    {
                        path.MoveTo(0, 0);
                        path.LineTo(outerRadius, 0);
                        path.ArcTo(outerRadius, outerRadius, areaAngle, SKPathArcSize.Small, SKPathDirection.Clockwise, point2.X, point2.Y);
                        path.LineTo(0, 0);
                        path.Close();
                        canvas.DrawPath(path, paint);
                    }
                    canvas.RotateDegrees(areaAngle, 0, 0);
                    if (j == 255)
                        break;
                    j++;
                }
            }

            using (var clip = new SKPath { FillType = SKPathFillType.EvenOdd })
            {
                clip.AddCircle(0, 0, innerRadius);
                clip.AddCircle(0, 0, outerRadius);
                canvas.ClipPath(clip, SKClipOperation.Difference, true);
                paint.Color = SKColors.White;
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
