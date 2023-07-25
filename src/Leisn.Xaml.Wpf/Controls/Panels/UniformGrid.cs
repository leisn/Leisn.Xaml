// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public int Rows { get => (int)GetValue(RowsProperty); set => SetValue(RowsProperty, value); }
        public static readonly DependencyProperty RowsProperty =
            System.Windows.Controls.Primitives.UniformGrid.RowsProperty.AddOwner(
                typeof(UniformGrid),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        [Bindable(true), Category("Layout")]
        public int Columns { get => (int)GetValue(ColumnsProperty); set => SetValue(ColumnsProperty, value); }
        public static readonly DependencyProperty ColumnsProperty =
             System.Windows.Controls.Primitives.UniformGrid.ColumnsProperty.AddOwner(
                typeof(UniformGrid),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

        [Bindable(true), Category("Layout")]
        public bool IsCurved { get { return (bool)GetValue(IsCurvedProperty); } set { SetValue(IsCurvedProperty, value); } }
        public static readonly DependencyProperty IsCurvedProperty =
            DependencyProperty.Register("IsCurved", typeof(bool), typeof(UniformGrid),
               new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.AffectsMeasure));

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
            int row, col, rowSpan, colSpan;
            double left, top, width, height;
            bool[,] _cells = new bool[_rows, _columns];
            var unArranged = new List<UIElement>();
            //先放置含有Grid.row的，再放置其他，已被占据的位置不能再放置，超出边界的叠在最后一格里
            foreach (UIElement child in InternalChildren)
            {
                if (child == null || child.Visibility == Visibility.Collapsed)
                {
                    continue;
                }
                row = Grid.GetRow(child);//start at 1
                col = Grid.GetColumn(child);
                rowSpan = Grid.GetRowSpan(child);
                colSpan = Grid.GetColumnSpan(child);
                if (row > 0 && col > 0)
                {
                    row -= 1;
                    col -= 1;
                    arrange(child);
                }
                else
                {
                    unArranged.Add(child);
                }
            }

            int index = 0;
            foreach (UIElement child in unArranged)
            {
                rowSpan = Grid.GetRowSpan(child);
                colSpan = Grid.GetColumnSpan(child);
            calcCell:
                if (Orientation == Orientation.Vertical)
                {
                    col = index / _rows;
                    row = index % _rows;
                }
                else
                {
                    row = index / _columns;
                    col = index % _columns;
                }
                index++;

                row = Math.Clamp(row, 0, _rows - 1);
                col = Math.Clamp(col, 0, _columns - 1);
                if (IsCurved)
                {
                    if (Orientation == Orientation.Vertical && col % 2 == 1)//even col
                    {
                        row = Math.Clamp(_rows - row - rowSpan, 0, _rows - 1);
                    }
                    else if (Orientation == Orientation.Horizontal && row % 2 == 1)//even row
                    {
                        col = Math.Clamp(_columns - col - colSpan, 0, _columns - 1);
                    }
                }
                if (_cells[row, col] && !(row == _rows - 1 && col == _columns - 1))
                {
                    goto calcCell;
                }
                arrange(child);
            }

            void arrange(UIElement child)
            {
                row = Math.Clamp(row, 0, _rows - 1);
                col = Math.Clamp(col, 0, _columns - 1);
                left = Padding.Left + col * cellWidth + col * hspace;
                top = Padding.Top + row * cellHeight + row * vspace;
                rowSpan = Math.Clamp(rowSpan, 1, _rows - row);
                colSpan = Math.Clamp(colSpan, 1, _columns - col);
                width = colSpan > 1 ? colSpan * cellWidth + (colSpan - 1) * hspace : cellWidth;
                height = rowSpan > 1 ? rowSpan * cellHeight + (rowSpan - 1) * vspace : cellHeight;
                child.Arrange(new Rect(left, top, width, height));
                for (int i = row; i < row + rowSpan; i++)
                {
                    for (int j = col; j < col + colSpan; j++)
                    {
                        _cells[i, j] = true;
                    }
                }
            }
            return finalSize;
        }
    }
}
