// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Leisn.Common.Media;
using Leisn.Xaml.Wpf.Extensions;

using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

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

    public enum ColorSpectrumStyle
    {
        Square, Disc
    }

    public sealed class ColorSpectrum : SKElement
    {
        static ColorSpectrum()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(ColorSpectrum), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(ColorSpectrum), new FrameworkPropertyMetadata(true));
        }

        private DpiScale _dpiScale;
        public ColorSpectrum()
        {
            IgnorePixelScaling = false;
            _dpiScale = VisualEx.GetDpiScale();
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            _dpiScale = newDpi;
            InvalidateVisual();
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
            add => AddHandler(SelectedHueChangedEvent, value);
            remove => RemoveHandler(SelectedHueChangedEvent, value);
        }

        public static readonly RoutedEvent SelectedHsvChangedEvent = EventManager.RegisterRoutedEvent(
         nameof(SelectedHsvChanged), RoutingStrategy.Bubble, typeof(SelectedHsvChangedEventHandler), typeof(ColorSpectrum));
        /// <summary>
        ///     An event fired when the hsv selection changes.
        /// </summary>
        [Category("Behavior")]
        public event SelectedHsvChangedEventHandler SelectedHsvChanged
        {
            add => AddHandler(SelectedHsvChangedEvent, value);
            remove => RemoveHandler(SelectedHsvChangedEvent, value);
        }

        public static readonly RoutedEvent SelectedRgbChangedEvent = EventManager.RegisterRoutedEvent(
           nameof(SelectedRgbChanged), RoutingStrategy.Bubble, typeof(SelectedRgbChangedEventHandler), typeof(ColorSpectrum));
        /// <summary>
        ///     An event fired when the color selection changes.
        /// </summary>
        [Category("Behavior")]
        public event SelectedRgbChangedEventHandler SelectedRgbChanged
        {
            add => AddHandler(SelectedRgbChangedEvent, value);
            remove => RemoveHandler(SelectedRgbChangedEvent, value);
        }
        #endregion

        #region public properties
        public double CircleLength
        {
            get => (double)GetValue(CircleLengthProperty);
            set => SetValue(CircleLengthProperty, value);
        }
        public static readonly DependencyProperty CircleLengthProperty =
            DependencyProperty.Register("CircleLength", typeof(double), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(.22d, FrameworkPropertyMetadataOptions.AffectsRender));
        public double Spacing
        {
            get => (double)GetValue(SpacingProperty);
            set => SetValue(SpacingProperty, value);
        }
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register("Spacing", typeof(double), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(.12d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double BorderWidth
        {
            get => (double)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }
        public static readonly DependencyProperty BorderWidthProperty =
            DependencyProperty.Register("BorderWidth", typeof(double), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsRender));

        public Color BorderColor
        {
            get => (Color)GetValue(BorderColorProperty);
            set => SetValue(BorderColorProperty, value);
        }
        public static readonly DependencyProperty BorderColorProperty =
            DependencyProperty.Register("BorderColor", typeof(Color), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(Colors.LightGray, FrameworkPropertyMetadataOptions.AffectsRender));

        public Hsv SelectedHue
        {
            get => (Hsv)GetValue(SelectedHueProperty);
            set => SetValue(SelectedHueProperty, value);
        }
        public static readonly DependencyProperty SelectedHueProperty =
            DependencyProperty.Register("SelectedHue", typeof(Hsv), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(new Hsv(0, 1, 1),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnSeletedHueChanged)));

        private static void OnSeletedHueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSpectrum cs = (ColorSpectrum)d;
            Hsv value = (Hsv)e.NewValue;
            cs.RaiseEvent(new SelectedHueChangedEventArgs((Hsv)e.OldValue, (Hsv)e.NewValue) { Source = cs });
            cs.SelectedHsv = new Hsv(value.H, cs.SelectedHsv.S, cs.SelectedHsv.V);
        }

        public Hsv SelectedHsv
        {
            get => (Hsv)GetValue(SelectedHsvProperty);
            set => SetValue(SelectedHsvProperty, value);
        }
        public static readonly DependencyProperty SelectedHsvProperty =
            DependencyProperty.Register("SelectedHsv", typeof(Hsv), typeof(ColorSpectrum),
                new FrameworkPropertyMetadata(new Hsv(0, 1, 1),
                    FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnSelectedHsvChanged)));

        private static void OnSelectedHsvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSpectrum cs = (ColorSpectrum)d;
            Hsv newHsv = (Hsv)e.NewValue;
            cs.RaiseEvent(new SelectedHsvChangedEventArgs((Hsv)e.OldValue, newHsv) { Source = cs });
            cs.SelectedHue = new Hsv(newHsv.H, 1, 1);
            cs.SelectedRgb = newHsv.ToRgb();
        }

        public Rgb SelectedRgb
        {
            get => (Rgb)GetValue(SelectedRgbProperty);
            set => SetValue(SelectedRgbProperty, value);
        }
        public static readonly DependencyProperty SelectedRgbProperty =
            DependencyProperty.Register("SelectedRgb", typeof(Rgb), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(new Rgb(255, 0, 0),
                     FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                     new PropertyChangedCallback(OnSelectedRgbChanged)));

        private static void OnSelectedRgbChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ColorSpectrum cs = (ColorSpectrum)d;
            Rgb value = (Rgb)e.NewValue;
            cs.RaiseEvent(new SelectedRgbChangedEventArgs((Rgb)e.OldValue, value) { Source = cs });
            if (Equals(value, cs.SelectedHsv.ToRgb()))
            {
                return;
            }

            cs.SelectedHsv = value.ToHsv();
        }

        public ColorSpectrumStyle SpectrumStyle
        {
            get => (ColorSpectrumStyle)GetValue(SpectrumStyleProperty);
            set => SetValue(SpectrumStyleProperty, value);
        }
        public static readonly DependencyProperty SpectrumStyleProperty =
            DependencyProperty.Register("SpectrumStyle", typeof(ColorSpectrumStyle), typeof(ColorSpectrum),
                 new FrameworkPropertyMetadata(ColorSpectrumStyle.Square, FrameworkPropertyMetadataOptions.AffectsRender));

        #endregion

        #region hue color maps
        private static Hsv HuePosToColor(double pos)
        {
            if (pos is < 0 or > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pos), $"{pos} not in [0,1]");
            }

            int colorSpace = 0;
            for (int i = 1; i < _colorHuePos.Length; i++)
            {
                if (pos <= _colorHuePos[i])
                {
                    colorSpace = i;
                    break;
                }
            }

            double posInArea = (pos - _colorHuePos[colorSpace - 1]) / (_colorHuePos[colorSpace] - _colorHuePos[colorSpace - 1]);
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
            Rgb color = hsv.ToRgb();
            double pos = -1;
            for (int i = 0; i < _colorHues.Length; i++)
            {
                if (color.R == _colorHues[i].Red && color.G == _colorHues[i].Green && color.B == _colorHues[i].Blue)
                {
                    return _colorHuePos[i];
                }
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

        private static readonly float[] _colorHuePos = new float[] { 0, 0.167f, 0.334f, 0.501f, 0.668f, 0.835f, 1 };
        private static readonly SKColor[] _colorHues = new SKColor[]
        {
            new SKColor(255, 0, 0),
            new SKColor(255, 255, 0),// 255 v 0
            new SKColor(0, 255, 0),// -v 255 0
            new SKColor(0, 255, 255),// 0 255 v
            new SKColor(0, 0, 255),//0 -v 255
            new SKColor(255, 0, 255),//v 0 255
            new SKColor(255, 0, 0)//255 0 -v
        };
        private static readonly float[] _spectrumEdgeColorPos
            = new float[] { 0, 0.125f, 0.25f, 0.375f, 0.5f, 0.625f, 0.75f, 0.875f, 1 };
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
            SKCanvas canvas = e.Surface.Canvas;
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
            using (SKShader shade = SKShader.CreateSweepGradient(new SKPoint(), _colorHues, _colorHuePos))
            using (SKPaint circlePaint = new() { IsStroke = false, IsAntialias = true, Shader = shade })
            using (SKPath path = new() { FillType = SKPathFillType.EvenOdd })
            {
                path.AddCircle(0, 0, _hueInnerRadius);
                path.AddCircle(0, 0, _hueOuterRadius);
                //canvas.ClipPath(clip, SKClipOperation.Intersect, true);
                canvas.DrawPath(path, circlePaint);
            }

            //draw hub picker
            using SKPaint pickerPaint = new()
            {
                IsStroke = true,
                IsAntialias = true,
                StrokeWidth = pickerWidth,
                Color = SelectedHue.ForegroundShouldBeLight() ? SKColors.White : SKColors.Black,
            };
            float hubPickerRadius = circleLength / 3.2f;
            float hubPickerPos = _hueInnerRadius + circleLength / 2;
            float selectedHubAngle = (float)HueColorToPos(SelectedHue) * 360;
            canvas.RotateDegrees(selectedHubAngle);
            canvas.DrawCircle(hubPickerPos, 0, hubPickerRadius, pickerPaint);

            canvas.Restore();
            #endregion

            #region draw color spectrum

            float colorPickerX = 0;
            float colorPickerY = 0;
            float spacing = (float)(Spacing > 1 ? Spacing : Spacing * _hueInnerRadius);
            _discSpectrumRadius = _hueInnerRadius - spacing - 1;
            if (SpectrumStyle == ColorSpectrumStyle.Disc)
            {
                //draw antialias,maybe a better way?

                using SKShader shade = SKShader.CreateSweepGradient(new SKPoint(),
                    new SKColor[] { SelectedHue.With(.5, 1).ToSKColor(), SelectedHue.With(1, 1).ToSKColor(),
                                    SelectedHue.With(1, .5).ToSKColor(), SelectedHue.With(1, 0).ToSKColor(),
                                    SelectedHue.With(.5, 0).ToSKColor(), SelectedHue.With(0, 0).ToSKColor(),
                                    SelectedHue.With(0, .5).ToSKColor(), SelectedHue.With(0, 1).ToSKColor(),
                                    SelectedHue.With(.5, 1).ToSKColor()}, _spectrumEdgeColorPos);
                using SKPaint edgePaint = new() { IsStroke = false, IsAntialias = true, Shader = shade };
                canvas.DrawCircle(0, 0, _discSpectrumRadius + 1, edgePaint);

                using SKImage spectrumImage = GenerateDiscSpectrumImage(_discSpectrumRadius * 2);
                canvas.DrawImage(spectrumImage, -_discSpectrumRadius, -_discSpectrumRadius);
                //cacl color picker location
                (double u, double v) = ShapeHelper.SquareToDiscMapping(2 * SelectedHsv.V - 1, 2 * SelectedHsv.S - 1);
                colorPickerX = (float)(u * _discSpectrumRadius);
                colorPickerY = (float)(v * _discSpectrumRadius);
            }
            else if (SpectrumStyle == ColorSpectrumStyle.Square)
            {
                //var innerSquareLength = (float)Math.Sqrt(innerRadius * innerRadius / 2);//内接正方形边长
                _squareSpectrumLength = (float)Math.Sqrt(2 * _discSpectrumRadius * _discSpectrumRadius);//边长
                float half = _squareSpectrumLength / 2;
                using SKImage spectrumImage = GenerateSquareSpectrumImage(_squareSpectrumLength);
                canvas.DrawImage(spectrumImage, -half, -half);
                colorPickerX = (float)(SelectedHsv.V * _squareSpectrumLength - half);
                colorPickerY = (float)(SelectedHsv.S * _squareSpectrumLength - half);
            }

            //draw color picker circle
            pickerPaint.Color = SelectedHsv.ForegroundShouldBeLight() ? SKColors.White : SKColors.Black;
            float colorPickerRadius = hubPickerRadius / 3 * 2;
            canvas.DrawCircle(colorPickerX, colorPickerY, colorPickerRadius, pickerPaint);
            _spectrumPickerRadius = colorPickerRadius + pickerWidth / 2;
            _spectrumPickerCenter = Vector2.Transform(
                    new Vector2(colorPickerX, colorPickerY),
                    Matrix3x2.CreateRotation((float)-Math.PI / 2))
                + new Vector2(_center.X, _center.Y);
            #endregion
            //draw broder
            if (BorderWidth > 0)
            {
                using SKPaint borderPaint = new() { IsStroke = true, StrokeWidth = (float)BorderWidth, IsAntialias = true };
                borderPaint.Color = BorderColor.ToSKColor();
                canvas.DrawCircle(0, 0, _hueInnerRadius, borderPaint);
                canvas.DrawCircle(0, 0, _hueOuterRadius, borderPaint);
                if (SpectrumStyle == ColorSpectrumStyle.Disc)
                {
                    canvas.DrawCircle(0, 0, _discSpectrumRadius, borderPaint);
                }
                else if (SpectrumStyle == ColorSpectrumStyle.Square)
                {
                    canvas.DrawRect(-_squareSpectrumLength / 2, -_squareSpectrumLength / 2,
                        _squareSpectrumLength, _squareSpectrumLength, borderPaint);
                }
            }
            canvas.Restore();
            base.OnPaintSurface(e);
        }

        private SKImage GenerateSquareSpectrumImage(float spectrumLength)
        {
            int length = (int)spectrumLength;
            byte[] bytes = new byte[length * length * 4];
            ushort h = SelectedHue.H;
            Parallel.For(0, length + 1, (y) => Parallel.For(0, length + 1, (x) =>
            {
                Hsv hsv = new(h, (double)y / length, (double)x / length);
                Rgb rgb = hsv.ToRgb();
                int col = (int)Math.Round(hsv.V * (length - 1));
                int row = (int)Math.Round(hsv.S * (length - 1));
                int bytePos = row * length * 4 + col * 4;
                bytes[bytePos] = rgb.R;
                bytes[bytePos + 1] = rgb.G;
                bytes[bytePos + 2] = rgb.B;
                bytes[bytePos + 3] = 0xFF;
            }));
            return SKImage.FromPixelCopy(new SKImageInfo(length, length, SKColorType.Rgba8888), bytes);
        }

        private SKImage GenerateDiscSpectrumImage(float spectrumLength)
        {
            int length = (int)spectrumLength;
            byte[] bytes = new byte[length * length * 4];
            ushort h = SelectedHue.H;
            Parallel.For(0, length + 1, (s) => Parallel.For(0, length + 1, (v) =>
            {
                Hsv hsv = new(h, (double)s / length, (double)v / length);
                Rgb rgb = hsv.ToRgb();
                (double x, double y) = ShapeHelper.SquareToDiscMapping(hsv.V * 2 - 1, hsv.S * 2 - 1);
                x = (x + 1) / 2;
                y = (y + 1) / 2;
                int col = (int)Math.Round(x * (length - 1));
                int row = (int)Math.Round(y * (length - 1));
                int bytePos = row * length * 4 + col * 4;
                bytes[bytePos] = rgb.R;
                bytes[bytePos + 1] = rgb.G;
                bytes[bytePos + 2] = rgb.B;
                bytes[bytePos + 3] = 0xFF;
            }));
            return SKImage.FromPixelCopy(new SKImageInfo(length, length, SKColorType.Rgba8888), bytes);
        }
        #endregion

        #region mouse actions
        private Point GetRealPoint(Point point)
            => new(point.X * _dpiScale.DpiScaleX, point.Y * _dpiScale.DpiScaleY);

        private bool _isMouseDown;
        private bool _isAdjustHue;
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.Source != this)
            {
                return;
            }

            _isAdjustHue = false;
            try
            {
                Point point = GetRealPoint(e.GetPosition(this));

                if (TrySetHue(point))
                {
                    _isAdjustHue = true;
                    _isMouseDown = true;
                    return;
                }
                if (SpectrumStyle == ColorSpectrumStyle.Disc)
                {
                    //圆形
                    if (TrySetInDiscSpectrum(point))
                    {
                        _isMouseDown = true;
                    }
                    return;
                }
                else if (SpectrumStyle == ColorSpectrumStyle.Square)
                {
                    //正方形
                    if (TrySetInSquareSpectrum(point))
                    {
                        _isMouseDown = true;
                    }
                }
            }
            finally
            {
                if (_isMouseDown)
                {
                    Keyboard.Focus(this);
                    CaptureMouse();
                }
            }

        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isMouseDown || e.Source != this)
            {
                return;
            }

            Point point = GetRealPoint(e.GetPosition(this));
            if (_isAdjustHue)
            {
                SetHue(point);
                return;
            }

            switch (SpectrumStyle)
            {
                case ColorSpectrumStyle.Square:
                    SetSquareColor(point);
                    break;
                case ColorSpectrumStyle.Disc:
                    SetDiscColor(point);
                    break;
                default:
                    break;
            }
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            _isMouseDown = false;
            ReleaseMouseCapture();
        }
        private bool TrySetHue(Point point)
        {
            (double distance, double _) = ShapeHelper.CartesianToPolar(point.X - _center.X, point.Y - _center.Y);
            if (distance >= _hueInnerRadius && distance <= _hueOuterRadius)
            {
                SetHue(point);
                return true;
            }
            return false;
        }
        private bool TrySetInSquareSpectrum(Point point)
        {
            float halfLenght = _squareSpectrumLength / 2;
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
            (double distance, double _) = ShapeHelper.CartesianToPolar(point.X - _center.X, point.Y - _center.Y);
            if (distance <= _discSpectrumRadius)
            {
                SetDiscColor(point);
                return true;
            }
            return IsPointInPicker(point);
        }

        private bool IsPointInPicker(Point point)
        {
            float distance = Vector2.Distance(
                _spectrumPickerCenter,
                new Vector2((float)point.X, (float)point.Y));
            return distance <= _spectrumPickerRadius;
        }

        private void SetHue(Point point)
        {
            (double _, double radians) = ShapeHelper.CartesianToPolar(point.X - _center.X, point.Y - _center.Y);
            int quadrant = ShapeHelper.Quadrant(point.X, point.Y, _center.X, _center.Y);
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
            double pos = Math.Clamp(radians / Math.PI / 2, 0, 1);
            SelectedHue = HuePosToColor(pos);
        }
        private void SetSquareColor(Point point)
        {
            float half = _squareSpectrumLength / 2;
            float px = (float)Math.Clamp((float)point.X, _center.X - half, _center.X + half);
            float py = (float)Math.Clamp((float)point.Y, _center.Y - half, _center.Y + half);
            Vector2 vector = new(px, py);
            vector -= new Vector2(_center.X, _center.Y);
            vector = Vector2.Transform(vector, Matrix3x2.CreateRotation((float)Math.PI / 2));
            float s = Math.Clamp((vector.Y + half) / _squareSpectrumLength, 0, 1);
            float v = Math.Clamp((vector.X + half) / _squareSpectrumLength, 0, 1);
            SelectedHsv = new Hsv(SelectedHue.H, s, v);
        }
        private void SetDiscColor(Point point)
        {
            Vector2 vector = new((float)point.X, (float)point.Y);
            vector -= new Vector2(_center.X, _center.Y);
            vector = Vector2.Transform(vector, Matrix3x2.CreateRotation((float)Math.PI / 2));
            //限制在边缘位置
            if (Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y) > _discSpectrumRadius)
            {
                vector = Vector2.Normalize(vector) * _discSpectrumRadius;
            }
            vector /= _discSpectrumRadius;//缩放到[-1,1]
            (double x, double y) = ShapeHelper.DiscToSquareMapping(vector.X, vector.Y);//映射到矩形
            x = Math.Clamp((x + 1) / 2, 0, 1);// 转换到[0,1]
            y = Math.Clamp((y + 1) / 2, 0, 1);
            SelectedHsv = new Hsv(SelectedHue.H, y, x);
        }

        #endregion


    }
}
