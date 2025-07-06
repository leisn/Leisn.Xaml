// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls
{
    public class StackPanel : SpacedPanelBase
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return new Size();
            }

            Size size = new();
            int count = 0;
            for (int i = 0; i < Children.Count; i++)
            {
                FrameworkElement child = (FrameworkElement)Children[i];
                if (child == null)
                {
                    continue;
                }

                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                count++;
                child.Measure(availableSize);
                Size childSize = child.DesiredSize;
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    size.Height += childSize.Height;
                    size.Width = Math.Max(size.Width, childSize.Width);
                }
                else
                {
                    size.Width += childSize.Width;
                    size.Height = Math.Max(size.Height, childSize.Height);
                }
            }
            if (count > 1)
            {
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                    size.Height += (count - 1) * FinalVerticalSpacing;
                else
                    size.Width += (count - 1) * FinalHorizontalSpacing;
            }
            size.Width += Padding.Left + Padding.Right;
            size.Height += Padding.Top + Padding.Bottom;
            return size;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
            {
                return new Size();
            }

            double finalWidth = finalSize.Width - (Padding.Left + Padding.Right);
            double finalHeight = finalSize.Height - (Padding.Top + Padding.Bottom);
            double left = Padding.Left;
            double top = Padding.Top;
            foreach (FrameworkElement? child in Children)
            {
                if (child == null)
                {
                    continue;
                }

                if (child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                Size childSize = child!.DesiredSize;
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    switch (child.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            break;
                        case HorizontalAlignment.Center:
                            left += (finalWidth - childSize.Width) / 2;
                            break;
                        case HorizontalAlignment.Right:
                            left += finalWidth - childSize.Width;
                            break;
                        case HorizontalAlignment.Stretch:
                        default:
                            childSize.Width = finalWidth;
                            break;
                    }
                    child.Arrange(new Rect(left, top, childSize.Width, childSize.Height));
                    left = Padding.Left;
                    top += childSize.Height + FinalVerticalSpacing;
                }
                else
                {
                    switch (child.VerticalAlignment)
                    {
                        case VerticalAlignment.Top:
                            break;
                        case VerticalAlignment.Center:
                            top += (finalHeight - childSize.Height) / 2;
                            break;
                        case VerticalAlignment.Bottom:
                            top += finalHeight - childSize.Height;
                            break;
                        case VerticalAlignment.Stretch:
                        default:
                            childSize.Height = finalHeight;
                            break;
                    }
                    child.Arrange(new Rect(left, top, childSize.Width, childSize.Height));
                    top = Padding.Top;
                    left += childSize.Width + FinalHorizontalSpacing;
                }
            }
            return finalSize;
        }
    }
}
