// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Leisn.Xaml.Wpf.Controls
{
    public class UniformGrid : SpacedPanelBase
    {
        static UniformGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UniformGrid), new FrameworkPropertyMetadata(typeof(UniformGrid)));
        }

        [Bindable(true), Category("Layout")]
        public int Rows
        {
            get => (int)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }
        public static readonly DependencyProperty RowsProperty =
            System.Windows.Controls.Primitives.UniformGrid.RowsProperty.AddOwner(
                typeof(UniformGrid),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        [Bindable(true), Category("Layout")]
        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }
        public static readonly DependencyProperty ColumnsProperty =
             System.Windows.Controls.Primitives.UniformGrid.ColumnsProperty.AddOwner(
                typeof(UniformGrid),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        //[Bindable(true), Category("Layout")]
        //public bool IsTortuous
        //{
        //    get { return (bool)GetValue(IsTortuousProperty); }
        //    set { SetValue(IsTortuousProperty, value); }
        //}
        //public static readonly DependencyProperty IsTortuousProperty =
        //    DependencyProperty.Register("IsTortuous", typeof(bool), typeof(SpacedUniformGrid),
        //       new FrameworkPropertyMetadata(false,
        //            FrameworkPropertyMetadataOptions.AffectsMeasure));

        private int _rows;
        private int _columns;

        private void UpdateCells()
        {
            _columns = Columns;
            _rows = Rows;
            if (_columns > 0 && _rows > 0) //value checked
            {
                return;
            }

            int childCount = 0;
            for (int i = 0, count = InternalChildren.Count; i < count; ++i)
            {
                UIElement child = InternalChildren[i];
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                childCount++;
            }
            if (_rows == 0)
            {
                if (_columns == 0)
                {
                    _rows = (int)Math.Sqrt(childCount);
                    if (_rows * _rows < childCount)
                    {
                        _rows++;
                    }

                    _columns = _rows;
                }
                else
                {
                    _rows = (childCount + _columns - 1) / _columns;
                }
            }
            else if (_columns == 0)
            {
                _columns = (childCount + _rows - 1) / _rows;
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {
            int count = InternalChildren.Count;
            if (count == 0)
            {
                return new Size();
            }

            UpdateCells();
            double hspace = FinalHorizontalSpacing;
            double vspace = FinalVerticalSpacing;
            double consWidth = (constraint.Width - Padding.Left - Padding.Right - (_columns - 1) * hspace) / _columns;
            double consHeight = (constraint.Height - Padding.Top - Padding.Bottom - (_rows - 1) * vspace) / _rows;
            Size childConstraint = new(consWidth > 0 ? consWidth : 0, consHeight > 0 ? consHeight : 0);
            double maxChildDesiredWidth = 0.0;
            double maxChildDesiredHeight = 0.0;

            //  Measure each child, keeping track of maximum desired width and height.
            for (int i = 0; i < count; ++i)
            {
                UIElement child = InternalChildren[i];
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                // Measure the child.
                child.Measure(childConstraint);
                Size childDesiredSize = child.DesiredSize;

                maxChildDesiredWidth = Math.Max(maxChildDesiredWidth, childDesiredSize.Width);
                maxChildDesiredHeight = Math.Max(maxChildDesiredHeight, childDesiredSize.Height);
            }

            double finalWidth = maxChildDesiredWidth * _columns + (_columns - 1) * hspace;
            double finalHeight = maxChildDesiredHeight * _rows + (_rows - 1) * vspace;
            finalWidth += Padding.Left + Padding.Right;
            finalHeight += Padding.Top + Padding.Bottom;
            finalWidth = Math.Max(0, finalWidth);
            finalHeight = Math.Max(0, finalHeight);
            return new Size(finalWidth, finalHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = InternalChildren.Count;
            if (count == 0)
            {
                return new Size();
            }

            double hspace = FinalHorizontalSpacing;
            double vspace = FinalVerticalSpacing;

            double cellWidth = (finalSize.Width - Padding.Left - Padding.Right - (_columns - 1) * hspace) / _columns;
            double cellHeight = (finalSize.Height - Padding.Top - Padding.Bottom - (_rows - 1) * vspace) / _rows;
            int row, col;
            double left, top, width, height;
            for (int i = 0; i < count; ++i)
            {
                UIElement child = InternalChildren[i];
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }

                if (Orientation == Orientation.Vertical)
                {
                    col = i / _rows;
                    row = i % _rows;
                }
                else
                {
                    row = i / _columns;
                    col = i % _columns;
                }

                row = row > _rows - 1 ? _rows - 1 : row; //keep in cells
                col = col > _columns - 1 ? _columns - 1 : col;

                //if (IsTortuous)
                //{
                //    if (Orientation == Orientation.Vertical && col % 2 == 1)//even col
                //        row = _rows - 1 - row;
                //    else if (Orientation == Orientation.Horizontal && row % 2 == 1)//even row
                //        col = _rows - 1 - col;
                //}

                int grow = Grid.GetRow(child);//start at 1
                grow = grow > 0 ? grow - 1 : row;
                int gcolumn = Grid.GetColumn(child);
                gcolumn = gcolumn > 0 ? gcolumn - 1 : col;


                left = Padding.Left + gcolumn * cellWidth + gcolumn * hspace;
                top = Padding.Top + grow * cellHeight + grow * vspace;

                int rowSpan = Grid.GetRowSpan(child);
                int columnSpan = Grid.GetColumnSpan(child);
                width = columnSpan > 1 ? columnSpan * cellWidth + (columnSpan - 1) * hspace : cellWidth;
                height = rowSpan > 1 ? rowSpan * cellHeight + (rowSpan - 1) * vspace : cellHeight;
                width = Math.Max(width, 0);
                height = Math.Max(height, 0);
                child.Arrange(new Rect(left, top, width, height));
            }
            return finalSize;
        }
    }
}
