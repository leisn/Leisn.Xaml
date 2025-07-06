// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls;

public class NodeCanvas : Canvas
{
    public NodeCanvas()
    {
        ClipToBounds = true;
    }

    public Point Offset
    {
        get { return (Point)GetValue(OffsetProperty); }
        set { SetValue(OffsetProperty, value); }
    }
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register("Offset", typeof(Point), typeof(NodeCanvas),
            new PropertyMetadata(new Point(), new PropertyChangedCallback(OnOffsetChanged)));

    private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var canvas = (NodeCanvas)d;
        var newPoint = (Point)e.NewValue;
        //Debug.WriteLine(newPoint);
        canvas.ApplyOffsetToChildren(newPoint - canvas._lastDragOffset);
    }

    private bool _isPressed;
    private Point _lastDragPoint;
    private Point _lastDragOffset;

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        if (e.LeftButton != MouseButtonState.Pressed && e.Source != this)
            return;
        _isPressed = true;
        UIElement depend = Window.GetWindow(this) ?? (UIElement)Parent ?? this;
        _lastDragPoint = e.GetPosition(depend);
        CaptureMouse();
        e.Handled = true;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (!_isPressed || e.LeftButton != MouseButtonState.Pressed)
        {
            OnMouseUp(null!);
            return;
        }
        Cursor = Cursors.SizeAll;
        UIElement depend = Window.GetWindow(this) ?? (UIElement)Parent ?? this;
        var mousePoint = e.GetPosition(depend);

        if (!mousePoint.Equals(_lastDragPoint))
        {
            double xDelta = mousePoint.X - _lastDragPoint.X;
            double yDelta = mousePoint.Y - _lastDragPoint.Y;

            Offset = new Point(_lastDragOffset.X + xDelta, _lastDragOffset.Y + yDelta);
            _lastDragPoint = mousePoint;
        }
        e.Handled = true;
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        _isPressed = false;
        ReleaseMouseCapture();
        Cursor = Cursors.Arrow;
    }

    private void ApplyOffsetToChildren(Vector offset)
    {
        foreach (UIElement child in Children)
        {
            var left = Canvas.GetLeft(child);
            var top = Canvas.GetTop(child);
            if (double.IsNaN(left)) left = 0;
            if (double.IsNaN(top)) top = 0;
            Canvas.SetLeft(child, left + offset.X);
            Canvas.SetTop(child, top + offset.Y);
        }
        _lastDragOffset = _lastDragOffset + offset;
    }

    #region zoom
    //private double _wheelOffset = 6;
    //public double MinZoomFactor
    //{
    //    get { return (double)GetValue(MinZoomFactorProperty); }
    //    set { SetValue(MinZoomFactorProperty, value); }
    //}
    //public static readonly DependencyProperty MinZoomFactorProperty =
    //    DependencyProperty.Register("MinZoomFactor", typeof(double), typeof(NodeCanvas), new PropertyMetadata(0.1d));
    //public double MaxZoomFactor
    //{
    //    get { return (double)GetValue(MaxZoomFactorProperty); }
    //    set { SetValue(MaxZoomFactorProperty, value); }
    //}
    //public static readonly DependencyProperty MaxZoomFactorProperty =
    //    DependencyProperty.Register("MaxZoomFactor", typeof(double), typeof(NodeCanvas), new PropertyMetadata(4d));


    //public double ZoomFactor
    //{
    //    get { return (double)GetValue(ZoomFactorProperty); }
    //    set { SetValue(ZoomFactorProperty, value); }
    //}
    //public static readonly DependencyProperty ZoomFactorProperty =
    //    DependencyProperty.Register("ZoomFactor", typeof(double), typeof(NodeCanvas),
    //        new PropertyMetadata(1d, new PropertyChangedCallback(OnZoomFactorChanged)));

    //private static void OnZoomFactorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //{
    //    var canvas = (NodeCanvas)d;
    //    canvas.Zoom(new Point(canvas.ActualWidth / 2, canvas.ActualHeight / 2), (double)e.OldValue, (double)e.NewValue);
    //}

    //protected override void OnMouseWheel(MouseWheelEventArgs e)
    //{
    //    e.Handled = true;
    //    var wheelOffset = _wheelOffset + (e.Delta / 120);
    //    double newScale = Math.Clamp(Math.Log(1 + (wheelOffset / 10d)) * 2d, MinZoomFactor, MaxZoomFactor);
    //    Point zoomCenter = e.GetPosition(this);
    //    Zoom(zoomCenter, ZoomFactor, newScale);
    //    ZoomFactor = newScale;
    //}

    //private void Zoom(Point zoomCenter, double oldScale, double newScale)
    //{
    //    var postToSelf = zoomCenter - Offset;
    //    var posScaled = postToSelf / oldScale * newScale;
    //    var offset = posScaled - postToSelf;
    //    var newOffset = Offset - offset;

    //    var scaleTrans = new ScaleTransform(newScale, newScale);
    //    this.RenderTransform = scaleTrans;
    //    Offset = newOffset;
    //    _wheelOffset = 10d * Math.Pow(Math.E, newScale / 2) - 10;
    //}


    #endregion
}
