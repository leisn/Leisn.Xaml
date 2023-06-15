// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using SkiaSharp.Views.Desktop;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using Leisn.Common.Media;
using System.Diagnostics;

namespace Leisn.Xaml.Wpf.Controls
{
    public class ColorSpectrum : SKElement
    {
        static ColorSpectrum()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(ColorSpectrum), new FrameworkPropertyMetadata(true));
        }

        public ColorSpectrum()
        {
            IgnorePixelScaling = true;
        }

        public double CircleLength
        {
            get { return (double)GetValue(CircleLengthProperty); }
            set { SetValue(CircleLengthProperty, value); }
        }
        public static readonly DependencyProperty CircleLengthProperty =
            DependencyProperty.Register("CircleLength", typeof(double), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(.28d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double BorderWidth
        {
            get { return (double)GetValue(BorderWidthProperty); }
            set { SetValue(BorderWidthProperty, value); }
        }
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Color), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(Colors.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color SelectedHue
        {
            get { return (Color)GetValue(SelectedHueProperty); }
            set { SetValue(SelectedHueProperty, value); }
        }
        public static readonly DependencyProperty SelectedHueProperty =
            DependencyProperty.Register("SelectedHue", typeof(Color), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(new Color { A = 255, R = 255 }, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(0.12d, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(new Color { A = 255, R = 255 }, FrameworkPropertyMetadataOptions.AffectsRender));

        #region color maps
        protected virtual Color PosToColor(double pos)
        {
            if (pos < 0 || pos > 1)
                throw new ArgumentOutOfRangeException(nameof(pos), $"{pos} not in [0,1]");
            int colorSpace = 0;
            for (int i = 1; i < _colorHuePos.Length; i++)
            {
                if (pos <= _colorHuePos[i])
                {
                    colorSpace = i;
                    break;
                }
            }

            var posInArea = (pos - _colorHuePos[colorSpace - 1]) / (_colorHuePos[colorSpace] - _colorHuePos[colorSpace - 1]);
            byte value = (byte)Math.Round(posInArea * 255);
            byte valueInverse = (byte)(255 - value);

            Color color = new() { A = 0xFF };
            switch (colorSpace)
            {
                case 1:
                    color.R = 255;
                    color.G = value;
                    color.B = 0;
                    break;
                case 2:
                    color.R = valueInverse;
                    color.G = 255;
                    color.B = 0;
                    break;
                case 3:
                    color.R = 0;
                    color.G = 255;
                    color.B = value;
                    break;
                case 4:
                    color.R = 0;
                    color.G = valueInverse;
                    color.B = 255;
                    break;
                case 5:
                    color.R = value;
                    color.G = 0;
                    color.B = 255;
                    break;
                case 6:
                    color.R = 255;
                    color.G = 0;
                    color.B = valueInverse;
                    break;
            }

            return color;
        }
        protected virtual double ColorToPos(Color color)
        {
            double pos = -1;
            for (int i = 0; i < _colorHues.Length; i++)
            {
                if (color.R == _colorHues[i].Red && color.G == _colorHues[i].Green && color.B == _colorHues[i].Blue)
                    return _colorHuePos[i];
            }
            if (color.R == 255 && color.B == 0)
            {
                pos = color.G / 255d * _colorHuePos[1];
            }
            else if (color.G == 255 && color.B == 0)
            {
                pos = (255 - color.R) / 255d * (_colorHuePos[2] - _colorHuePos[1]) + _colorHuePos[1];
            }
            else if (color.R == 0 && color.G == 255)
            {
                pos = color.B / 255d * (_colorHuePos[3] - _colorHuePos[2]) + _colorHuePos[2];
            }
            else if (color.R == 0 && color.B == 255)
            {
                pos = (255 - color.G) / 255d * (_colorHuePos[4] - _colorHuePos[3]) + _colorHuePos[3];
            }
            else if (color.G == 0 && color.B == 255)
            {
                pos = color.R / 255d * (_colorHuePos[5] - _colorHuePos[4]) + _colorHuePos[4];
            }
            else if (color.R == 255 && color.G == 0)
            {
                pos = (255 - color.B) / 255d * (_colorHuePos[6] - _colorHuePos[5]) + _colorHuePos[5];
            }
            return (float)pos;
        }


        static readonly float[] _colorHuePos = new float[] { 0, 0.167f, 0.334f, 0.501f, 0.668f, 0.835f, 1 };
        static readonly SKColor[] _colorHues = new SKColor[]
        {
            new SKColor(255, 0, 0),
            new SKColor(255, 255, 0),// 255 v 0
            new SKColor(0, 255, 0),// -v 255 0
            new SKColor(0, 255, 255),// 0 255 v
            new SKColor(0, 0, 255),//0 -v 255
            new SKColor(255, 0, 255),//v 0 255
            new SKColor(255, 0, 0)//255 0 -v
        };
        #endregion


        Point GetCenter() => new Point(ActualWidth / 2, ActualHeight / 2);
        double GetOuterRadius() => (Math.Min(ActualWidth, ActualHeight) - 10) / 2;
        double GetPickerRadius()
        {
            var outerRadius = GetOuterRadius();
            return outerRadius - (CircleLength > 1 ? CircleLength : CircleLength * outerRadius) / 2;
        }
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            var center = new SKPoint(e.Info.Rect.MidX, e.Info.Rect.MidY);
            float outerRadius = (float)((Math.Min(e.Info.Width, e.Info.Height) - 10) / 2 - BorderWidth);
            float circleLength = (float)(CircleLength > 1 ? CircleLength : CircleLength * outerRadius);
            float innerRadius = outerRadius - circleLength;
            float pickerWidth = Math.Max(circleLength / 12, 2f);
            #region draw color cicle
            canvas.Save();
            canvas.Translate(center);
            canvas.RotateDegrees(-90);
            //draw cicle
            using (var circlePaint = new SKPaint { IsStroke = false, IsAntialias = true })
            using (var shade = SKShader.CreateSweepGradient(new SKPoint(), _colorHues, _colorHuePos))
            using (var path = new SKPath { FillType = SKPathFillType.EvenOdd })
            {
                path.AddCircle(0, 0, innerRadius);
                path.AddCircle(0, 0, outerRadius);
                //canvas.ClipPath(clip, SKClipOperation.Intersect, true);
                circlePaint.Shader = shade;
                canvas.DrawPath(path, circlePaint);
            }

            //draw hub picker
            using var hubPickerPaint = new SKPaint
            {
                IsStroke = true,
                IsAntialias = true,
                StrokeWidth = pickerWidth,
                Color = SelectedHue.ForegroundShouldBeLight() ? SKColors.White : SKColors.Black,
            };
            float hubPickerRadius = circleLength / 3.2f;
            float hubPickerPos = innerRadius + circleLength / 2;
            var selectedHubAngle = (float)ColorToPos(SelectedHue) * 360;
            canvas.RotateDegrees(selectedHubAngle);
            canvas.DrawCircle(hubPickerPos, 0, hubPickerRadius, hubPickerPaint);

            canvas.Restore();
            #endregion

            #region draw color spectrum
            canvas.Save();
            canvas.Translate(center);
            var spacing = (float)(Spacing > 1 ? Spacing : Spacing * innerRadius);
            var spectrumRadius = innerRadius - spacing;
            //draw spectrum
            using var spectrumPanint = new SKPaint { IsAntialias = true, Color = SKColors.Transparent };
            canvas.DrawBitmap(GenerateSpectrum((int)(spectrumRadius * 2)), -spectrumRadius, -spectrumRadius, spectrumPanint);
            //draw color picker circle
            using var colorPickerPanint = new SKPaint
            {
                IsAntialias = true,
                IsStroke = true,
                StrokeWidth = pickerWidth,
                Color = SelectedColor.ForegroundShouldBeLight() ? SKColors.White : SKColors.Black,
            };
            float colorPickerRadius = hubPickerRadius / 3 * 2;
            float colorPickerX = 0;
            float colorPickerY = 0;
            canvas.DrawCircle(colorPickerX, colorPickerY, colorPickerRadius, colorPickerPanint);
            canvas.Restore();
            #endregion
            //draw broder
            if (BorderWidth > 0)
            {
                using var borderPaint = new SKPaint { IsStroke = true, StrokeWidth = (float)BorderWidth, IsAntialias = true };
                borderPaint.Color = BorderColor.ToSKColor();
                canvas.DrawCircle(0, 0, spectrumRadius, borderPaint);
                canvas.DrawCircle(0, 0, innerRadius, borderPaint);
                canvas.DrawCircle(0, 0, outerRadius, borderPaint);
            }

            base.OnPaintSurface(e);
        }

        private SKBitmap GenerateSpectrum(int length)
        {
            var bitmap = new SKBitmap(length, length, SKColorType.Rgba8888, SKAlphaType.Premul);
            var hsv = new Rgb(SelectedHue.R, SelectedHue.G, SelectedHue.B).ToHsv();
            hsv.S = hsv.S * 2 - 1;
            hsv.V = hsv.V * 2 - 1;
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < length; x++)
                {
                    var uv = SquareToDiscMapping(x, y);

                    var color = SKColors.Blue;
                    if (!double.IsNaN(uv.X) && !double.IsNaN(uv.Y))
                        bitmap.SetPixel((int)uv.X, (int)uv.Y, color);
                }
            }
            return bitmap;
        }

        //圆形到正方形点位映射
        static Point DiscToSquareMapping(double u, double v)
        {
            double u2 = u * u;
            double v2 = v * v;
            double twosqrt2 = 2 * Math.Sqrt(2);
            double subtermx = 2 + u2 - v2;
            double subtermy = 2 - u2 + v2;
            double termx1 = subtermx + u * twosqrt2;
            double termx2 = subtermx - u * twosqrt2;
            double termy1 = subtermy + v * twosqrt2;
            double termy2 = subtermy - v * twosqrt2;
            double x = 0.5 * Math.Sqrt(termx1) - 0.5 * Math.Sqrt(termx2);
            double y = 0.5 * Math.Sqrt(termy1) - 0.5 * Math.Sqrt(termy2);
            return new Point(x, y);
        }
        //长方形到圆形点位映射
        static Point SquareToDiscMapping(double x, double y)
        {
            var u = x * Math.Sqrt(1 - y * y / 2);
            var v = y * Math.Sqrt(1 - x * x / 2);
            return new Point(u, v);
        }


        #region mouse actions
        private bool _isMouseDown;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _isMouseDown = true;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown)
                return;
            var point = e.GetPosition(this);
            var center = GetCenter();
            var v1 = new Vector2((float)center.X, 0);
            var v2 = new Vector2((float)point.X, (float)point.Y);
            var vc = new Vector2((float)center.X, (float)center.Y);
            var ab = Vector2.Distance(v1, vc);
            var ac = Vector2.Distance(v2, vc);
            var bc = Vector2.Distance(v1, v2);
            var radians = Math.Acos((ab * ab + ac * ac - bc * bc) / (2 * ab * ac));
            var quadrant = Quadrant(center, point);
            switch (quadrant)
            {
                case 1:
                case 2:
                    break;
                case 3:
                case 4:
                    radians = Math.PI * 2 - radians;
                    break;
            }
            var pos = Math.Clamp(radians / Math.PI / 2, 0, 1);
            SelectedHue = PosToColor(pos);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;

        }

        public static int Quadrant(Point orgin, Point target)
        {
            if (orgin.X < target.X)
                return orgin.Y > target.Y ? 1 : 2;
            else
                return orgin.Y > target.Y ? 4 : 3;
        }
        #endregion
    }
}
