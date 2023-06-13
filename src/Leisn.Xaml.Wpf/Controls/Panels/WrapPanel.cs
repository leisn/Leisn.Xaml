// By Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Windows;

namespace Leisn.Xaml.Wpf.Controls.Panels
{
    public class WrapPanel : SpacedPanelBase
    {
        static WrapPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WrapPanel), new FrameworkPropertyMetadata(typeof(WrapPanel)));
        }

        /// <summary>
        /// 每几个换一次行或列，默认0（自动换）
        /// </summary>
        public int WrapEachItems
        {
            get => (int)GetValue(WrapEachItemsProperty);
            set => SetValue(WrapEachItemsProperty, value);
        }
        public static readonly DependencyProperty WrapEachItemsProperty =
            DependencyProperty.Register("WrapEachItems", typeof(int), typeof(WrapPanel),
                new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        private readonly List<double> _rowOrColumMaxLenght = new();
        protected override Size MeasureOverride(Size availableSize)
        {
            _rowOrColumMaxLenght.Clear();
            if (Children.Count == 0)
            {
                return new Size();
            }

            double width = 0, height = 0;
            double wrapWidth = 0, wrapHeight = 0;
            int row = 0, column = 0;

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
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    bool needWrap = (WrapEachItems != 0 && row != 0 && row % WrapEachItems == 0)
                                        || wrapHeight + childSize.Height + vspace > availableSize.Height;
                    if (needWrap)
                    {
                        _rowOrColumMaxLenght.Add(wrapWidth);
                        width += wrapWidth + (column == 0 ? 0 : hspace);
                        height = Math.Max(wrapHeight, height);
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
                    bool needWrap = (WrapEachItems != 0 && column != 0 && column % WrapEachItems == 0)
                                         || wrapWidth + childSize.Width + hspace > availableSize.Width;
                    if (needWrap)
                    {
                        _rowOrColumMaxLenght.Add(wrapHeight);
                        height += wrapHeight + (row == 0 ? 0 : vspace);
                        width = Math.Max(wrapWidth, width);
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
                width += wrapWidth + hspace;
                height = Math.Max(wrapHeight, height);
            }
            else
            {
                _rowOrColumMaxLenght.Add(wrapHeight);
                height += wrapHeight + vspace;
                width = Math.Max(wrapWidth, width);
            }
            return new Size(Padding.Left + Padding.Right + width, Padding.Top + Padding.Bottom + height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = Children.Count;
            if (count == 0)
            {
                return new Size();
            }

            double wrapWidth = 0, wrapHeight = 0;
            int row = 0, column = 0;
            double left = Padding.Left, top = Padding.Top;

            double hspace = FinalHorizontalSpacing;
            double vspace = FinalVerticalSpacing;

            if (finalSize.Height == 0)
            {
                finalSize.Height = DesiredSize.Height;
            }

            if (finalSize.Width == 0)
            {
                finalSize.Width = DesiredSize.Width;
            }

            foreach (FrameworkElement? child in Children)
            {
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                Size childSize = child!.DesiredSize;
                if (Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    bool needWrap = (WrapEachItems != 0 && row != 0 && row % WrapEachItems == 0)
                                        || (wrapHeight + childSize.Height + vspace > finalSize.Height && row != 0);
                    double maxWidth = _rowOrColumMaxLenght[column];
                    if (needWrap)
                    {
                        column++;
                        left += maxWidth + hspace;
                        row = 0;
                        top = Padding.Top;
                        wrapHeight = 0;
                    }

                    child.Arrange(new Rect(left, top, maxWidth, childSize.Height));
                    wrapHeight += childSize.Height + (row == 0 ? 0 : vspace);
                    row++;
                    top += childSize.Height + (row == 0 ? 0 : vspace);
                }
                else
                {
                    bool needWrap = (WrapEachItems != 0 && column != 0 && column % WrapEachItems == 0)
                                         || (wrapWidth + childSize.Width + hspace > finalSize.Width && column != 0);
                    double maxHeight = _rowOrColumMaxLenght[row];
                    if (needWrap)
                    {
                        column = 0;
                        left = Padding.Left;
                        row++;
                        top += maxHeight + vspace;
                        wrapWidth = 0;
                    }
                    child.Arrange(new Rect(left, top, childSize.Width, maxHeight));
                    wrapWidth += childSize.Width + (column == 0 ? 0 : hspace);
                    column++;
                    left += childSize.Width + (column == 0 ? 0 : hspace);
                }
            }


            Size arranged = new(double.IsInfinity(finalSize.Width) ? DesiredSize.Width : finalSize.Width,
                double.IsInfinity(finalSize.Height) ? DesiredSize.Height : finalSize.Height);
            return arranged;
        }
    }
}
