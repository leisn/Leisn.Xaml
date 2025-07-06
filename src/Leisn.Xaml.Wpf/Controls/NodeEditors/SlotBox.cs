using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls;

public class SlotBox : Control
{
    public static EllipseGeometry Circle { get; } = new EllipseGeometry(new Rect(0, 0, 11, 11));
    public static RectangleGeometry Rectangle { get; } = new RectangleGeometry(new Rect(0, 0, 10, 10));
    static SlotBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(SlotBox), new FrameworkPropertyMetadata(typeof(SlotBox)));
    }
    public Geometry Shape
    {
        get { return (Geometry)GetValue(ShapeProperty); }
        set { SetValue(ShapeProperty, value); }
    }
    public static readonly DependencyProperty ShapeProperty =
        DependencyProperty.Register("Shape", typeof(Geometry), typeof(SlotBox), new PropertyMetadata(Circle));

    public double StrokeWidth
    {
        get { return (double)GetValue(StrokeWidthProperty); }
        set { SetValue(StrokeWidthProperty, value); }
    }
    public static readonly DependencyProperty StrokeWidthProperty =
        DependencyProperty.Register("StrokeWidth", typeof(double), typeof(SlotBox), new PropertyMetadata(1d));

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
        e.Handled = true;
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
    }
}

