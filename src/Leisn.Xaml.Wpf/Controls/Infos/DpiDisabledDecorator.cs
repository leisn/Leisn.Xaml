// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace Leisn.Xaml.Wpf.Controls
{
    public class DpiDisabledDecorator : Decorator
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var pSource = PresentationSource.FromVisual(this);
            Matrix matrix = pSource.CompositionTarget.TransformFromDevice;
            var transform = new MatrixTransform(matrix);
            if (transform.CanFreeze)
                transform.Freeze();
            LayoutTransform = transform;
        }
    }
}
