// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    /// <summary>
    /// 单行或单列布局，内容始终填充整个可用区域
    /// </summary>
    public class StackFillPanel : Panel
    {
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackFillPanel),
                new PropertyMetadata(Orientation.Horizontal, PropertyChangedCallback));

        public double Space
        {
            get => (double)GetValue(SpaceProperty);
            set => SetValue(SpaceProperty, value);
        }

        public static readonly DependencyProperty SpaceProperty =
            DependencyProperty.Register("Space", typeof(double), typeof(StackFillPanel),
                new PropertyMetadata(0d, PropertyChangedCallback));

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register("Padding", typeof(Thickness), typeof(StackFillPanel),
                new PropertyMetadata(new Thickness(), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StackFillPanel sfp)
            {
                sfp.InvalidateMeasure();
            }
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            Size result = new(0, 0);

            int count = Children.Count;
            if (count == 0)
            {
                return result;
            }

            Size maxChildSize = new();
            Size desiredSize = new();
            double totalSpace = (count - 1) * Space;

            foreach (UIElement? item in Children)
            {
                if (item == null)
                {
                    continue;
                }

                item.Measure(availableSize);
                maxChildSize.Width = Math.Max(item.DesiredSize.Width, maxChildSize.Width);
                maxChildSize.Height = Math.Max(item.DesiredSize.Height, maxChildSize.Height);
                desiredSize.Width += item.DesiredSize.Width;
                desiredSize.Height += item.DesiredSize.Height;
            }
            if (Orientation == Orientation.Vertical)
            {
                double vspace = Padding.Top + Padding.Bottom + totalSpace;
                double maxHeight = maxChildSize.Height * count + vspace;
                double desiredHeight = desiredSize.Height + vspace;
                result.Width = maxChildSize.Width + Padding.Left + Padding.Right;
                result.Height = !double.IsInfinity(availableSize.Height) && availableSize.Height >= maxHeight ? availableSize.Height : desiredHeight;
            }
            else
            {
                double hspace = Padding.Left + Padding.Right + totalSpace;
                double maxWidth = maxChildSize.Width * count + hspace;
                double desiredWidth = desiredSize.Width + hspace;
                result.Height = maxChildSize.Height + Padding.Top + Padding.Bottom;
                result.Width = !double.IsInfinity(availableSize.Width) && availableSize.Width >= maxWidth ? availableSize.Width : desiredWidth;
            }

            return result;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = Children.Count;
            if (count == 0)
            {
                return new Size(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            }

            double totalSpace = (count - 1) * Space;
            double left = Padding.Left, top = Padding.Top,
                width = DesiredSize.Width, height = DesiredSize.Height;
            Size usedSize = finalSize;

            if (Orientation == Orientation.Vertical)
            {
                width = Math.Max(usedSize.Width, width);
                //空间不够用
                if (height > finalSize.Height)
                {
                    foreach (UIElement? child in Children)
                    {
                        if (child == null)
                        {
                            continue;
                        }

                        child!.Arrange(new Rect(left, top, width, child.DesiredSize.Height));
                        top += child.DesiredSize.Height + Space;
                    }
                    usedSize.Height = top - Space;
                }
                else
                {
                    height = (finalSize.Height - Padding.Top - Padding.Bottom - totalSpace) / count;
                    foreach (UIElement? child in Children)
                    {
                        if (child == null)
                        {
                            continue;
                        }

                        child.Arrange(new Rect(left, top, width, height));
                        top += Space + height;
                    }
                }
                usedSize.Width = width;
            }
            else
            {
                height = Math.Max(usedSize.Height, height);
                //空间不够用
                if (width > finalSize.Width)
                {
                    foreach (UIElement? child in Children)
                    {
                        if (child == null)
                        {
                            continue;
                        }

                        child.Arrange(new Rect(left, top, child.DesiredSize.Width, height));
                        left += child.DesiredSize.Width + Space;
                    }
                    usedSize.Width = left - Space;
                }
                else
                {
                    width = (finalSize.Width - Padding.Left - Padding.Right - totalSpace) / count;
                    foreach (UIElement? child in Children)
                    {
                        if (child == null)
                        {
                            continue;
                        }

                        child.Arrange(new Rect(left, top, width, height));
                        left += Space + width;
                    }
                }
                usedSize.Height = height;
            }
            return usedSize;
        }
    }
}
