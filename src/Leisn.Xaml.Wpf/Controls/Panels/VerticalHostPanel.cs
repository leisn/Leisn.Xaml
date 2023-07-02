using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Leisn.Xaml.Wpf.Controls.Panels
{
    public class VerticalHostPanel : Panel
    {
        private double _offset;

        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {

        }
    }
}
