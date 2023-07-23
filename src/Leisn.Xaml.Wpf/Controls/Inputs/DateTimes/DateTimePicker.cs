// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    public class DateTimePicker : Control
    {
        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }
    }
}
