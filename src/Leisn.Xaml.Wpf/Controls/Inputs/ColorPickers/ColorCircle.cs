// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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

        public ColorCircle()
        {
            IgnorePixelScaling = true;
        }

        public double CircleLength
        {
            get { return (double)GetValue(CircleLengthProperty); }
            set { SetValue(CircleLengthProperty, value); }
        }
        public static readonly DependencyProperty CircleLengthProperty =
            DependencyProperty.Register("CircleLength", typeof(double), typeof(ColorCircle),
                new FrameworkPropertyMetadata(.28d, FrameworkPropertyMetadataOptions.AffectsRender));

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

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorCircle),
                 new FrameworkPropertyMetadata(new Color { A = 255, R = 255 }, FrameworkPropertyMetadataOptions.AffectsRender));

        #region color maps
        protected virtual Color PosToColor(double pos)
        {
            if (pos < 0 || pos > 1)
                throw new ArgumentOutOfRangeException(nameof(pos), $"{pos} not in [0,1]");
            int colorSpace = 0;
            for (int i = 1; i < _colorPos.Length; i++)
            {
                if (pos <= _colorPos[i])
                {
                    colorSpace = i;
                    break;
                }
            }

            var posInArea = (pos - _colorPos[colorSpace - 1]) / (_colorPos[colorSpace] - _colorPos[colorSpace - 1]);
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
            for (int i = 0; i < _colors.Length; i++)
            {
                if (color.R == _colors[i].Red && color.G == _colors[i].Green && color.B == _colors[i].Blue)
                    return _colorPos[i];
            }
            if (color.R == 255 && color.B == 0)
            {
                pos = color.G / 255d * _colorPos[1];
            }
            else if (color.G == 255 && color.B == 0)
            {
                pos = (255 - color.R) / 255d * (_colorPos[2] - _colorPos[1]) + _colorPos[1];
            }
            else if (color.R == 0 && color.G == 255)
            {
                pos = color.B / 255d * (_colorPos[3] - _colorPos[2]) + _colorPos[2];
            }
            else if (color.R == 0 && color.B == 255)
            {
                pos = (255 - color.G) / 255d * (_colorPos[4] - _colorPos[3]) + _colorPos[3];
            }
            else if (color.G == 0 && color.B == 255)
            {
                pos = color.R / 255d * (_colorPos[5] - _colorPos[4]) + _colorPos[4];
            }
            else if (color.R == 255 && color.G == 0)
            {
                pos = (255 - color.B) / 255d * (_colorPos[6] - _colorPos[5]) + _colorPos[5];
            }
            return (float)pos;
        }


        static readonly float[] _colorPos = new float[] { 0, 0.167f, 0.334f, 0.501f, 0.668f, 0.835f, 1 };
        static readonly SKColor[] _colors = new SKColor[]
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
            canvas.Save();
            canvas.Translate(center);
            canvas.RotateDegrees(-90);
            //draw colors
            using (var circlePaint = new SKPaint { IsStroke = false, IsAntialias = true })
            using (var shade = SKShader.CreateSweepGradient(new SKPoint(), _colors, _colorPos))
            using (var path = new SKPath { FillType = SKPathFillType.EvenOdd })
            {
                path.AddCircle(0, 0, innerRadius);
                path.AddCircle(0, 0, outerRadius);
                //canvas.ClipPath(clip, SKClipOperation.Intersect, true);
                circlePaint.Shader = shade;
                canvas.DrawPath(path, circlePaint);
            }

            //draw picker
            using var pickerPaint = new SKPaint
            {
                IsStroke = true,
                Color = SelectedColor.ForegroundShouldBeLight() ? SKColors.White : SKColors.Black,
                StrokeWidth = Math.Max(circleLength / 12, 2f),
                IsAntialias = true,
            };
            float pickerRadius = circleLength / 4;
            float pickerPos = innerRadius + circleLength / 2;
            var selectedAngle = (float)ColorToPos(SelectedColor) * 360;
            canvas.RotateDegrees(selectedAngle);
            canvas.DrawCircle(new SKPoint(pickerPos, 0), pickerRadius, pickerPaint);

            //draw broder
            if (BorderWidth > 0)
            {
                using var borderPaint = new SKPaint { IsStroke = true, StrokeWidth = (float)BorderWidth, IsAntialias = true };
                borderPaint.Color = BorderColor.ToSKColor();
                canvas.DrawCircle(0, 0, innerRadius, borderPaint);
                canvas.DrawCircle(0, 0, outerRadius, borderPaint);
            }

            canvas.Restore();
            base.OnPaintSurface(e);
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
            SelectedColor = PosToColor(pos);
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
