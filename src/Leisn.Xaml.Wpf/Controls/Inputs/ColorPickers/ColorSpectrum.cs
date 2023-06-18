// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using SkiaSharp.Views.Desktop;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using Leisn.Common.Media;
using System.ComponentModel;
using System.Diagnostics;

namespace Leisn.Xaml.Wpf.Controls
{
    #region events delegate
    public delegate void SelectedHueChangedEventHandler(object sender, SelectedHueChangedEventArgs e);
    public class SelectedHueChangedEventArgs : RoutedEventArgs
    {
        public Hsv OldValue { get; }
        public Hsv NewValue { get; }
        public SelectedHueChangedEventArgs(Hsv oldValue, Hsv newValue)
        {
            RoutedEvent = ColorSpectrum.SelectedHueChangedEvent;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    public delegate void SelectedHsvChangedEventHandler(object sender, SelectedHsvChangedEventArgs e);
    public class SelectedHsvChangedEventArgs : RoutedEventArgs
    {
        public Hsv OldValue { get; }
        public Hsv NewValue { get; }
        public SelectedHsvChangedEventArgs(Hsv oldValue, Hsv newValue)
        {
            RoutedEvent = ColorSpectrum.SelectedHsvChangedEvent;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    public delegate void SelectedRgbChangedEventHandler(object sender, SelectedRgbChangedEventArgs e);
    public class SelectedRgbChangedEventArgs : RoutedEventArgs
    {
        public Rgb OldValue { get; }
        public Rgb NewValue { get; }
        public SelectedRgbChangedEventArgs(Rgb oldValue, Rgb newValue)
        {
            RoutedEvent = ColorSpectrum.SelectedRgbChangedEvent;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

    #endregion

    public sealed class ColorSpectrum : SKElement
    {
        public ColorSpectrum()
        {
            IgnorePixelScaling = true;
        }

        static ColorSpectrum()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(ColorSpectrum), new FrameworkPropertyMetadata(true));
        }

        #region public events
        public static readonly RoutedEvent SelectedHueChangedEvent = EventManager.RegisterRoutedEvent(
            nameof(SelectedHueChanged), RoutingStrategy.Bubble, typeof(SelectedHueChangedEventHandler), typeof(ColorSpectrum));
        /// <summary>
        ///     An event fired when the hue selection changes.
        /// </summary>
        [Category("Behavior")]
        public event SelectedHueChangedEventHandler SelectedHueChanged
        {
            add { AddHandler(SelectedHueChangedEvent, value); }
            remove { RemoveHandler(SelectedHueChangedEvent, value); }
        }

        public static readonly RoutedEvent SelectedHsvChangedEvent = EventManager.RegisterRoutedEvent(
         nameof(SelectedHsvChanged), RoutingStrategy.Bubble, typeof(SelectedHsvChangedEventHandler), typeof(ColorSpectrum));
        /// <summary>
        ///     An event fired when the hsv selection changes.
        /// </summary>
        [Category("Behavior")]
        public event SelectedHsvChangedEventHandler SelectedHsvChanged
        {
            add { AddHandler(SelectedHsvChangedEvent, value); }
            remove { RemoveHandler(SelectedHsvChangedEvent, value); }
        }

        public static readonly RoutedEvent SelectedRgbChangedEvent = EventManager.RegisterRoutedEvent(
           nameof(SelectedRgbChanged), RoutingStrategy.Bubble, typeof(SelectedRgbChangedEventHandler), typeof(ColorSpectrum));
        /// <summary>
        ///     An event fired when the color selection changes.
        /// </summary>
        [Category("Behavior")]
        public event SelectedRgbChangedEventHandler SelectedRgbChanged
        {
            add { AddHandler(SelectedRgbChangedEvent, value); }
            remove { RemoveHandler(SelectedRgbChangedEvent, value); }
        }
        #endregion

        #region public properties
        public double CircleLength
        {
            get { return (double)GetValue(CircleLengthProperty); }
            set { SetValue(CircleLengthProperty, value); }
        }
        public static readonly DependencyProperty CircleLengthProperty =
            DependencyProperty.Register("CircleLength", typeof(double), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(.22d, FrameworkPropertyMetadataOptions.AffectsRender));
        public double Spacing
        {
            get { return (double)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(.12d, FrameworkPropertyMetadataOptions.AffectsRender));

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

        public Hsv SelectedHue
        {
            get { return (Hsv)GetValue(SelectedHueProperty); }
            set { SetValue(SelectedHueProperty, value); }
        }
        public static readonly DependencyProperty SelectedHueProperty =
            DependencyProperty.Register("SelectedHue", typeof(Hsv), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(new Hsv(0, 1, 1),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnSeletedHueChanged)));

        private static void OnSeletedHueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cs = (ColorSpectrum)d;
            var value = (Hsv)e.NewValue;
            cs.RaiseEvent(new SelectedHueChangedEventArgs((Hsv)e.OldValue, (Hsv)e.NewValue) { Source = cs });
            cs.SelectedHsv = new Hsv(value.H, cs.SelectedHsv.S, cs.SelectedHsv.V);
        }

        public Hsv SelectedHsv
        {
            get { return (Hsv)GetValue(SelectedHsvProperty); }
            set { SetValue(SelectedHsvProperty, value); }
        }
        public static readonly DependencyProperty SelectedHsvProperty =
            DependencyProperty.Register("SelectedHsv", typeof(Hsv), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(new Hsv(0, 1, 1),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnSelectedHsvChanged)));

        private static void OnSelectedHsvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cs = (ColorSpectrum)d;
            cs.RaiseEvent(new SelectedHsvChangedEventArgs((Hsv)e.OldValue, (Hsv)e.NewValue) { Source = cs });
            cs.SelectedRgb = ((Hsv)e.NewValue).ToRgb();
        }

        public Rgb SelectedRgb
        {
            get { return (Rgb)GetValue(SelectedRgbProperty); }
            set { SetValue(SelectedRgbProperty, value); }
        }
        public static readonly DependencyProperty SelectedRgbProperty =
            DependencyProperty.Register("SelectedRgb", typeof(Rgb), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(new Rgb(255, 0, 0),
                     FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                     new PropertyChangedCallback(OnSelectedRgbChanged)));

        private static void OnSelectedRgbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cs = (ColorSpectrum)d;
            var value = (Rgb)e.NewValue;
            cs.RaiseEvent(new SelectedRgbChangedEventArgs((Rgb)e.OldValue, value) { Source = cs });
            if (Equals(value, cs.SelectedHsv.ToRgb()))
                return;
            cs.SelectedHsv = value.ToHsv();
        }

        public bool IsDiscSpectrum
        {
            get { return (bool)GetValue(IsDiscSpectrumProperty); }
            set { SetValue(IsDiscSpectrumProperty, value); }
        }

        public static readonly DependencyProperty IsDiscSpectrumProperty =
            DependencyProperty.Register("IsDiscSpectrum", typeof(bool), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        #region hue color maps 
        private static Hsv HuePosToColor(double pos)
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

            Rgb color = new();
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

            return color.ToHsv();
        }
        private static double HueColorToPos(Hsv hsv)
        {
            var color = hsv.ToRgb();
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

        #region draw and vars
        private SKPoint _center;
        private float _hueOuterRadius;
        private float _hueInnerRadius;
        private float _discSpectrumRadius;
        private float _squareSpectrumLength;
        private float _spectrumPickerRadius;
        private Vector2 _spectrumPickerCenter;
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear();
            _center = new SKPoint(e.Info.Rect.MidX, e.Info.Rect.MidY);
            _hueOuterRadius = (float)((Math.Min(e.Info.Width, e.Info.Height) - 10) / 2 - BorderWidth);
            float circleLength = (float)(CircleLength > 1 ? CircleLength : CircleLength * _hueOuterRadius);
            _hueInnerRadius = _hueOuterRadius - circleLength;
            float pickerWidth = Math.Max(circleLength / 12, 2f);
            #region draw color cicle
            canvas.Save();
            canvas.Translate(_center);
            canvas.RotateDegrees(-90);
            canvas.Save();
            //draw cicle
            using (var circlePaint = new SKPaint { IsStroke = false, IsAntialias = true })
            using (var shade = SKShader.CreateSweepGradient(new SKPoint(), _colorHues, _colorHuePos))
            using (var path = new SKPath { FillType = SKPathFillType.EvenOdd })
            {
                path.AddCircle(0, 0, _hueInnerRadius);
                path.AddCircle(0, 0, _hueOuterRadius);
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
            float hubPickerPos = _hueInnerRadius + circleLength / 2;
            var selectedHubAngle = (float)HueColorToPos(SelectedHue) * 360;
            canvas.RotateDegrees(selectedHubAngle);
            canvas.DrawCircle(hubPickerPos, 0, hubPickerRadius, hubPickerPaint);

            canvas.Restore();
            #endregion

            #region draw color spectrum
            //canvas.Save();
            //canvas.Translate(center);
            float colorPickerX = 0;
            float colorPickerY = 0;
            var spacing = (float)(Spacing > 1 ? Spacing : Spacing * _hueInnerRadius);
            _discSpectrumRadius = _hueInnerRadius - spacing;
            if (IsDiscSpectrum)
            {
                using var spectrumImage = GenerateDiscSpectrumImage((int)(_discSpectrumRadius * 2));
                canvas.DrawImage(spectrumImage, -_discSpectrumRadius, -_discSpectrumRadius);
                var (u, v) = ShapeHelper.SquareToDiscMapping(2 * SelectedHsv.V - 1, 2 * SelectedHsv.S - 1);
                colorPickerX = (float)(u * _discSpectrumRadius);
                colorPickerY = (float)(v * _discSpectrumRadius);
            }
            else
            {
                //var innerSquareLength = (float)Math.Sqrt(innerRadius * innerRadius / 2);//内接正方形边长
                _squareSpectrumLength = (float)Math.Sqrt(2 * _discSpectrumRadius * _discSpectrumRadius);//边长
                var half = _squareSpectrumLength / 2;
                using var spectrumImage = GenerateSquareSpectrumImage((int)_squareSpectrumLength);
                canvas.DrawImage(spectrumImage, -half, -half);
                colorPickerX = (float)(SelectedHsv.V * _squareSpectrumLength - half);
                colorPickerY = (float)(SelectedHsv.S * _squareSpectrumLength - half);
            }

            //draw color picker circle
            using var colorPickerPaint = new SKPaint
            {
                IsAntialias = true,
                IsStroke = true,
                StrokeWidth = pickerWidth,
                Color = SelectedHsv.ForegroundShouldBeLight() ? SKColors.White : SKColors.Black,
            };
            var colorPickerRadius = hubPickerRadius / 3 * 2;
            canvas.DrawCircle(colorPickerX, colorPickerY, colorPickerRadius, colorPickerPaint);

            _spectrumPickerRadius = colorPickerRadius + pickerWidth / 2;
            _spectrumPickerCenter = Vector2.Transform(
                    new Vector2(colorPickerX, colorPickerY),
                    Matrix3x2.CreateRotation((float)-Math.PI / 2))
                + new Vector2(_center.X, _center.Y);
            #endregion
            //draw broder
            if (BorderWidth > 0)
            {
                using var borderPaint = new SKPaint { IsStroke = true, StrokeWidth = (float)BorderWidth, IsAntialias = true };
                borderPaint.Color = BorderColor.ToSKColor();
                canvas.DrawCircle(0, 0, _hueInnerRadius, borderPaint);
                canvas.DrawCircle(0, 0, _hueOuterRadius, borderPaint);
                if (IsDiscSpectrum)
                {
                    canvas.DrawCircle(0, 0, _discSpectrumRadius, borderPaint);
                }
                else
                {
                    canvas.DrawRect(-_squareSpectrumLength / 2, -_squareSpectrumLength / 2,
                        _squareSpectrumLength, _squareSpectrumLength, borderPaint);
                }
            }
            canvas.Restore();
            base.OnPaintSurface(e);
        }

        private SKImage GenerateSquareSpectrumImage(int length)
        {
            var bytes = new byte[length * length * 4];
            var selectedHue = SelectedHue;
            Parallel.For(0, length, (i) =>
            {
                Rgb rgb;
                Hsv hsv = new(selectedHue.H, (double)i / length, 0);
                int bytePos;
                for (int j = 0; j < length; j++)
                {
                    hsv.V = (double)j / length;
                    rgb = hsv.ToRgb();

                    bytePos = i * length * 4 + j * 4;
                    bytes[bytePos] = rgb.R;
                    bytes[bytePos + 1] = rgb.G;
                    bytes[bytePos + 2] = rgb.B;
                    bytes[bytePos + 3] = 0xFF;
                }
            });
            return SKImage.FromPixelCopy(new SKImageInfo(length, length, SKColorType.Rgba8888), bytes);
        }
        private SKImage GenerateDiscSpectrumImage(int length)
        {
            var multipleLength = length * 2;//放大倍数，用于边缘抗锯齿，可能需要更好的实现方式
            var radius = multipleLength / 2;
            var bytes = new byte[multipleLength * multipleLength * 4];
            var selectedHue = SelectedHue;
            Parallel.For(0, multipleLength, (i) =>
            {
                int bytePos;
                Rgb rgb;
                Hsv hsv = new(selectedHue.H, (double)i / multipleLength, 0);
                double u, v;
                int row, col;
                for (int j = 0; j < multipleLength; j++)
                {
                    hsv.V = (double)j / multipleLength;
                    rgb = hsv.ToRgb();
                    (u, v) = ShapeHelper.SquareToDiscMapping(hsv.V * 2 - 1, hsv.S * 2 - 1);
                    col = (int)(radius * u + radius);
                    row = (int)(radius * v + radius);

                    bytePos = row * multipleLength * 4 + col * 4;
                    bytes[bytePos] = rgb.R;
                    bytes[bytePos + 1] = rgb.G;
                    bytes[bytePos + 2] = rgb.B;
                    bytes[bytePos + 3] = 0xFF;
                }
            });
            using var image2 = SKImage.FromPixelCopy(new SKImageInfo(multipleLength, multipleLength, SKColorType.Rgba8888), bytes);
            var image = SKImage.Create(new SKImageInfo(length, length, SKColorType.Rgba8888));
            image2.ScalePixels(image.PeekPixels(), SKFilterQuality.High);
            return image;
        }
        #endregion

        #region mouse actions
        private bool _isMouseDown;
        private bool _isAdjustHue;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source != this)
                return;
            _isAdjustHue = false;
            try
            {
                var point = e.GetPosition(this);
                if (TrySetHue(point))
                {
                    _isAdjustHue = true;
                    _isMouseDown = true;
                    return;
                }
                if (IsDiscSpectrum)
                {
                    //圆形
                    if (TrySetInDiscSpectrum(point))
                        _isMouseDown = true;
                    return;
                }
                //正方形
                if (TrySetInSquareSpectrum(point))
                    _isMouseDown = true;
            }
            finally
            {
                if (_isMouseDown)
                    CaptureMouse();
            }

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown || e.Source != this)
                return;
            var point = e.GetPosition(this);
            if (_isAdjustHue)
            {
                SetHue(point);
                return;
            }
            if (IsDiscSpectrum)
            {
                SetDiscColor(point);
                return;
            }
            SetSquareColor(point);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            ReleaseMouseCapture();
        }
        private bool TrySetHue(Point point)
        {
            var (distance, _) = ShapeHelper.CartesianToPolar(point.X - _center.X, point.Y - _center.Y);
            if (distance >= _hueInnerRadius && distance <= _hueOuterRadius)
            {
                SetHue(point);
                return true;
            }
            return false;
        }
        private bool TrySetInSquareSpectrum(Point point)
        {
            var halfLenght = _squareSpectrumLength / 2;
            if (Math.Abs(point.X - _center.X) <= halfLenght
                && Math.Abs(point.Y - _center.Y) <= halfLenght)
            {
                SetSquareColor(point);
                return true;
            }
            return IsPointInPicker(point);
        }
        private bool TrySetInDiscSpectrum(Point point)
        {
            var (distance, _) = ShapeHelper.CartesianToPolar(point.X - _center.X, point.Y - _center.Y);
            if (distance <= _discSpectrumRadius)
            {
                SetDiscColor(point);
                return true;
            }
            return IsPointInPicker(point);
        }

        private bool IsPointInPicker(Point point)
        {
            var distance = Vector2.Distance(
                _spectrumPickerCenter,
                new Vector2((float)point.X, (float)point.Y));
            return distance <= _spectrumPickerRadius;
        }

        private void SetHue(Point point)
        {
            var (_, radians) = ShapeHelper.CartesianToPolar(point.X - _center.X, point.Y - _center.Y);
            var quadrant = ShapeHelper.Quadrant(point.X, point.Y, _center.X, _center.Y);
            switch (quadrant)
            {
                case 1:
                case 2:
                case 4:
                    radians = Math.PI / 2 + radians;
                    break;
                case 3:
                    radians = Math.PI * 2.5 + radians;
                    break;
            }
            var pos = Math.Clamp(radians / Math.PI / 2, 0, 1);
            SelectedHue = HuePosToColor(pos);
        }
        private void SetSquareColor(Point point)
        {
            float half = _squareSpectrumLength / 2;
            var px = (float)Math.Clamp((float)point.X, _center.X - half, _center.X + half);
            var py = (float)Math.Clamp((float)point.Y, _center.Y - half, _center.Y + half);
            var vector = new Vector2(px, py);
            vector -= new Vector2(_center.X, _center.Y);
            vector = Vector2.Transform(vector, Matrix3x2.CreateRotation((float)Math.PI / 2));
            float s = (vector.Y + half) / _squareSpectrumLength;
            float v = (vector.X + half) / _squareSpectrumLength;
            SelectedHsv = new Hsv(SelectedHue.H, s, v);
        }
        private void SetDiscColor(Point point)
        {
            var vector = new Vector2((float)point.X, (float)point.Y);
            vector -= new Vector2(_center.X, _center.Y);
            vector = Vector2.Transform(vector, Matrix3x2.CreateRotation((float)Math.PI / 2));
            //限制在边缘位置
            if (Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y) > _discSpectrumRadius)
            {
                vector = Vector2.Normalize(vector) * _discSpectrumRadius;
            }
            vector /= _discSpectrumRadius;//缩放到[-1,1]
            var (x, y) = ShapeHelper.DiscToSquareMapping(vector.X, vector.Y);
            x = (x + 1) / 2;// 转换到[0,1]
            y = (y + 1) / 2;
            SelectedHsv = new Hsv(SelectedHue.H, y, x);
        }

        #endregion


    }
}
