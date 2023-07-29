// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls
{
    public class WrapPanel : SpacedPanelBase
    {
        /// <summary>
        /// 每几个换一次行或列，默认0（自动换）
        /// </summary>
        public int WrapLength
        {
            get => (int)GetValue(WrapLengthProperty);
            set => SetValue(WrapLengthProperty, value);
        }
        public static readonly DependencyProperty WrapLengthProperty =
            DependencyProperty.Register("WrapLength", typeof(int), typeof(WrapPanel),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private readonly List<double> _rowOrColumMaxLenght = new();
        protected override Size MeasureOverride(Size availableSize)
        {
            _rowOrColumMaxLenght.Clear();
            if (Children.Count == 0 || availableSize.IsEmpty)
            {
                return new Size();
            }

            availableSize.Width -= Padding.Left + Padding.Right;
            availableSize.Height -= Padding.Top + Padding.Bottom;

            double wrapWidth = 0, wrapHeight = 0;
            int row = 0, column = 0;
            Size finalSize = new();

            double hspace = FinalHorizontalSpacing;
            double vspace = FinalVerticalSpacing;

            foreach (FrameworkElement? child in Children)
            {
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                child!.Measure(availableSize);
                Size childSize = child.DesiredSize;

                if (Orientation == System.Windows.Controls.Orientation.Vertical)//垂直方向
                {
                    bool needWrap = (WrapLength != 0 && row != 0 && row % WrapLength == 0)
                                        || wrapHeight + childSize.Height + vspace > availableSize.Height;
                    if (needWrap)
                    {
                        _rowOrColumMaxLenght.Add(wrapWidth);
                        finalSize.Width += wrapWidth + (column == 0 ? 0 : hspace);
                        finalSize.Height = Math.Max(wrapHeight, finalSize.Height);
                        wrapWidth = 0;
                        wrapHeight = 0;
                        row = 0;
                        column++;
                    }
                    wrapHeight += childSize.Height + (row == 0 ? 0 : vspace);
                    wrapWidth = Math.Max(wrapWidth, childSize.Width);
                    row++;
                }
                else
                {
                    bool needWrap = (WrapLength != 0 && column != 0 && column % WrapLength == 0)
                                         || wrapWidth + childSize.Width + hspace > availableSize.Width;
                    if (needWrap)
                    {
                        _rowOrColumMaxLenght.Add(wrapHeight);
                        finalSize.Height += wrapHeight + (row == 0 ? 0 : vspace);
                        finalSize.Width = Math.Max(wrapWidth, finalSize.Width);
                        wrapWidth = 0;
                        wrapHeight = 0;
                        column = 0;
                        row++;
                    }
                    wrapWidth += childSize.Width + (column == 0 ? 0 : hspace);
                    wrapHeight = Math.Max(wrapHeight, childSize.Height);
                    column++;
                }
            }
            if (Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                _rowOrColumMaxLenght.Add(wrapWidth);
                finalSize.Width += wrapWidth + (column > 0 ? hspace : 0);
                finalSize.Height = Math.Max(wrapHeight, finalSize.Height);
            }
            else
            {
                _rowOrColumMaxLenght.Add(wrapHeight);
                finalSize.Height += wrapHeight + (row > 0 ? vspace : 0);
                finalSize.Width = Math.Max(wrapWidth, finalSize.Width);

            }
            finalSize.Height += Padding.Top + Padding.Bottom;
            finalSize.Width += Padding.Left + Padding.Right;

            if (double.IsInfinity(availableSize.Width) && Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                finalSize.Height = Math.Max(finalSize.Height, availableSize.Height);
            }
            if (double.IsInfinity(availableSize.Height) && Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                finalSize.Width = Math.Max(finalSize.Width, availableSize.Width);
            }
            return finalSize;
        }

        protected override Size ArrangeOverride(Size regionSize)
        {
            int count = Children.Count;
            if (count == 0 || regionSize.IsEmpty)
            {
                return new Size();
            }

            int row = 0, column = 0;
            double wrapWidth = 0, wrapHeight = 0, max;
            double left = Padding.Left, top = Padding.Top, right = 0, bottom = 0;

            double hspace = FinalHorizontalSpacing;
            double vspace = FinalVerticalSpacing;

            Size paddingedSize = new(regionSize.Width - Padding.Left - Padding.Right,
                regionSize.Height - Padding.Top - Padding.Bottom);
            Rect rect;
            foreach (FrameworkElement? child in Children)
            {
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                Size childSize = child!.DesiredSize;
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    childSize.Height = Math.Min(childSize.Height, paddingedSize.Height);
                    bool needWrap = (WrapLength != 0 && row != 0 && row % WrapLength == 0)
                                        || wrapHeight + childSize.Height + vspace > paddingedSize.Height && row != 0;
                    max = _rowOrColumMaxLenght[column];
                    if (needWrap)
                    {
                        column++;
                        left += max + hspace;
                        row = 0;
                        top = Padding.Top;
                        wrapHeight = 0;
                        max = _rowOrColumMaxLenght[column];
                    }
                    rect = new Rect(left, top, max, childSize.Height);
                    right = Math.Max(right, rect.Right);
                    bottom = Math.Max(bottom, rect.Bottom);
                    child.Arrange(rect);
                    wrapHeight += childSize.Height + (row == 0 ? 0 : vspace);
                    row++;
                    top += childSize.Height + (row == 0 ? 0 : vspace);
                }
                else
                {
                    childSize.Width = Math.Min(childSize.Width, paddingedSize.Width);
                    bool needWrap = (WrapLength != 0 && column != 0 && column % WrapLength == 0)
                                         || wrapWidth + childSize.Width + hspace > paddingedSize.Width && column != 0;
                    max = _rowOrColumMaxLenght[row];
                    if (needWrap)
                    {
                        column = 0;
                        left = Padding.Left;
                        row++;
                        top += max + vspace;
                        wrapWidth = 0;
                        max = _rowOrColumMaxLenght[row];
                    }
                    rect = new Rect(left, top, childSize.Width, max);
                    right = Math.Max(right, rect.Right);
                    bottom = Math.Max(bottom, rect.Bottom);
                    child.Arrange(rect);
                    wrapWidth += childSize.Width + (column == 0 ? 0 : hspace);
                    column++;
                    left += childSize.Width + (column == 0 ? 0 : hspace);
                }
            }

            return new Size(Math.Max(right + Padding.Right, regionSize.Width),
                Math.Max(bottom + Padding.Bottom, regionSize.Height));
        }
    }
}
