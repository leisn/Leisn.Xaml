using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls;

public class RoulettePanel : Panel
{

    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }
    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register("Radius", typeof(double), typeof(RoulettePanel),
            new FrameworkPropertyMetadata(200d, FrameworkPropertyMetadataOptions.AffectsMeasure));

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
            return new Size(0, 0);

        var p = MathF.PI * 2 / Children.Count;
        float radians = 0;
        var rootCenter = new Vector2(0, 0);
        var point = new Vector2(rootCenter.X + (float)Radius, rootCenter.Y);
        double left = double.PositiveInfinity, top = double.PositiveInfinity,
              right = double.NegativeInfinity, bottom = double.NegativeInfinity;
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            if (child.Visibility == Visibility.Collapsed) continue;
            child.Measure(availableSize);
            var size = new Vector2((float)child.DesiredSize.Width / 2, (float)child.DesiredSize.Height / 2);

            var matrix = Matrix3x2.CreateRotation(radians, rootCenter);
            var center = Vector2.Transform(point, matrix);
            left = Math.Min(center.X - size.X, left);
            top = Math.Min(center.Y - size.Y, top);
            right = Math.Max(center.X + size.X, right);
            bottom = Math.Max(center.Y + size.Y, bottom);
            radians += p;
        }
        var halfWidth = Math.Max(Math.Abs(left), right);
        var halfHeight = Math.Max(Math.Abs(top), bottom);
        return new Size(halfWidth * 2, halfHeight * 2);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
            return new Size(0, 0);
        var p = MathF.PI * 2 / Children.Count;
        var hp = p / 2;
        float radians = 0;
        var rootCenter = new Vector2((float)(finalSize.Width / 2), (float)(finalSize.Height / 2));
        var point = new Vector2(rootCenter.X + (float)Radius, rootCenter.Y);
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            if (child.Visibility == Visibility.Collapsed) continue;
            var size = new Vector2((float)child.DesiredSize.Width / 2, (float)child.DesiredSize.Height / 2);

            var matrix = Matrix3x2.CreateRotation(radians, rootCenter);
            var center = Vector2.Transform(point, matrix);
            Rect rect = RectEx.Create(center.X - size.X, center.Y - size.Y, center.X + size.X, center.Y + size.Y);
            if (child is RouletteItem item)
            {
                item.StartRadians = radians - hp;
                item.EndRadians = radians + hp;
            }
            child.Arrange(rect);
            radians += p;
        }
        return finalSize;
    }
}
