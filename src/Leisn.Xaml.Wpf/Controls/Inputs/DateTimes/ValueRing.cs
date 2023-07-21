using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

using Leisn.Xaml.Wpf.Extensions;

using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    internal class ValueRing : SKElement
    {
        static ValueRing()
        {
            SnapsToDevicePixelsProperty.OverrideMetadata(typeof(ValueRing), new FrameworkPropertyMetadata(true));
            FocusableProperty.OverrideMetadata(typeof(ValueRing), new FrameworkPropertyMetadata(true));
        }

        private DpiScale _dpiScale;
        public ValueRing()
        {
            IgnorePixelScaling = false;
            _dpiScale = VisualEx.GetDpiScale();
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            _dpiScale = newDpi;
            InvalidateVisual();
        }

        #region value properties
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(ValueRing));
        [Category("Behavior")]
        public event RoutedPropertyChangedEventHandler<double> ValueChanged { add { AddHandler(ValueChangedEvent, value); } remove { RemoveHandler(ValueChangedEvent, value); } }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register("Minimum", typeof(double), typeof(ValueRing),
                new FrameworkPropertyMetadata(0d,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    new PropertyChangedCallback(OnMinimumPropertyChanged)));

        private static void OnMinimumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueRing ctrl = (ValueRing)d;
            ctrl.CoerceValue(MaximumProperty);
            ctrl.CoerceValue(ValueProperty);
            ctrl.OnMinimumChanged((double)e.OldValue, (double)e.NewValue);
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(ValueRing),
                     new FrameworkPropertyMetadata(1d,
                         FrameworkPropertyMetadataOptions.AffectsRender,
                         new PropertyChangedCallback(OnMaximumPropertyChanged),
                         new CoerceValueCallback(CoerceMaximum)));

        private static void OnMaximumPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueRing ctrl = (ValueRing)d;
            ctrl.CoerceValue(ValueProperty);
            ctrl.OnMaximumChanged((double)e.OldValue, (double)e.NewValue);
        }

        private static object CoerceMaximum(DependencyObject d, object value)
        {
            ValueRing ctrl = (ValueRing)d;
            double min = ctrl.Minimum;
            if ((double)value < min)
            {
                return min;
            }
            return value;
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ValueRing),
                     new FrameworkPropertyMetadata(0d,
                        FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.AffectsRender,
                        new PropertyChangedCallback(OnValuePropertyChanged),
                        new CoerceValueCallback(CoerceValueToRange)));

        private static object CoerceValueToRange(DependencyObject d, object value)
        {
            ValueRing ctrl = (ValueRing)d;
            double min = ctrl.Minimum;
            double v = (double)value;
            if (v < min)
            {
                return min;
            }

            double max = ctrl.Maximum;
            if (v > max)
            {
                return max;
            }
            return value;
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ValueRing ctrl = (ValueRing)d;
            ctrl.OnValueChanged((double)e.OldValue, (double)e.NewValue);
        }

        protected virtual void OnMinimumChanged(double oldMinimum, double newMinimum)
        {
        }
        protected virtual void OnMaximumChanged(double oldMaximum, double newMaximum)
        {
        }
        protected virtual void OnValueChanged(double oldValue, double newValue)
        {
            RaiseEvent(new RoutedPropertyChangedEventArgs<double>(oldValue, newValue, ValueChangedEvent));
        }
        #endregion

        #region appearance properties

        public double RingRadius
        {
            get { return (double)GetValue(RingRadiusProperty); }
            set { SetValue(RingRadiusProperty, value); }
        }
        public static readonly DependencyProperty RingRadiusProperty =
            DependencyProperty.Register("RingRadius", typeof(double), typeof(ValueRing),
                new FrameworkPropertyMetadata(.5d, FrameworkPropertyMetadataOptions.AffectsRender));

        public double RingWidth
        {
            get { return (double)GetValue(RingWidthProperty); }
            set { SetValue(RingWidthProperty, value); }
        }
        public static readonly DependencyProperty RingWidthProperty =
            DependencyProperty.Register("RingWidth", typeof(double), typeof(ValueRing),
                new FrameworkPropertyMetadata(10d, FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }
        public static readonly DependencyProperty ForegroundProperty =
            Control.ForegroundProperty.AddOwner(typeof(ValueRing));
        #endregion

        #region action properties
        #endregion

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();
            var size = Math.Min(e.Info.Width, e.Info.Height);
            var xOffset = e.Info.Width - size;
            var yOffset = e.Info.Height - size;
            canvas.Translate(xOffset, yOffset);
            float center = size / 2f;
            var ringWidth = (float)(RingWidth > 1 ? RingWidth : RingWidth * center);
            var ringRadius = (float)(RingRadius > 1 ? RingRadius : RingRadius * center);

            using (var circlePaint = new SKPaint() { IsStroke = false })
            using (SKPath path = new() { FillType = SKPathFillType.EvenOdd })
            {
                path.AddCircle(center, center, ringRadius - ringWidth / 2);
                path.AddCircle(center, center, ringRadius + ringWidth / 2);
                canvas.DrawPath(path, circlePaint);
            }

            base.OnPaintSurface(e);
        }
    }
}
