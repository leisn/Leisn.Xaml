using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

using Leisn.Common.Media;

namespace Leisn.Xaml.Wpf.Controls;

/// <summary>
/// 轮盘选择器
/// </summary>
public class Roulette : ListBox
{
    const string PART_IndicatorName = "PART_Indicator";
    private FrameworkElement _indicator = null!;
    static Roulette()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Roulette), new FrameworkPropertyMetadata(typeof(Roulette)));
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _indicator = (FrameworkElement)GetTemplateChild(PART_IndicatorName);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is RouletteItem;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new RouletteItem();
    }

    private bool _isPressed;
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        _isPressed = true;
        CaptureMouse();
    }
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        try
        {
            base.OnMouseLeftButtonUp(e);
            if (!_isPressed)
                return;
            var point = e.GetPosition(this);
            SelectedIndex = GetMouseOnItemIndex(point, out _);
        }
        finally
        {
            _isPressed = false;
            ReleaseMouseCapture();
        }
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            _isPressed = false;
            return;
        }

        base.OnPreviewKeyDown(e);
    }

    private int GetMouseOnItemIndex(Point point, out double angle)
    {
        var count = Items.Count;
        var center = new Point(ActualWidth / 2, ActualHeight / 2);
        var polar = ShapeHelper.CartesianToPolar(point.X - center.X, point.Y - center.Y);
        var radians = polar.Radians;
        angle = radians * 180 / Math.PI;
        int quadrant = ShapeHelper.Quadrant(point.X, point.Y, center.X, center.Y);
        switch (quadrant)
        {
            case 1:
            case 2:
                break;
            case 4:
            case 3:
                radians = Math.PI * 2 + radians;
                break;
        }

        var pi2 = Math.PI * 2;
        for (int i = 0; i < count; i++)
        {
            var container = ItemContainerGenerator.ContainerFromIndex(i) as RouletteItem;
            if (container == null) continue;
            if (container.StartRadians < 0)
            {
                var start = container.StartRadians + pi2;
                var end = container.EndRadians + pi2;
                if (radians < end && radians > start)
                    return i;
            }
            if (radians < container.EndRadians && radians > container.StartRadians)
            {
                return i;
            }
        }
        return -1;
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        if (!HasItems)
            return;
        var count = Items.Count;
        var point = e.GetPosition(this);
        var index = GetMouseOnItemIndex(point, out var angle);
        var left = Canvas.GetLeft(_indicator) / -2;
        _indicator.RenderTransform = new RotateTransform(angle, left, 0);
        for (int i = 0; i < count; i++)
        {
            if (ItemContainerGenerator.ContainerFromIndex(i) is not RouletteItem container)
                continue;
            container.IsHighlight = index == i;
        }
    }
}

public class RouletteItem : ListBoxItem
{
    static RouletteItem()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(RouletteItem), new FrameworkPropertyMetadata(typeof(RouletteItem)));
    }

    public double StartRadians { get; internal set; }
    public double EndRadians { get; internal set; }

    public bool IsHighlight
    {
        get { return (bool)GetValue(IsHighlightProperty); }
        set { SetValue(IsHighlightProperty, value); }
    }

    public static readonly DependencyProperty IsHighlightProperty =
        DependencyProperty.Register("IsHighlight", typeof(bool), typeof(RouletteItem),
            new FrameworkPropertyMetadata(false));
}
