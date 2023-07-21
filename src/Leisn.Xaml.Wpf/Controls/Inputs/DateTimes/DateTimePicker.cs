// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;

using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_CanvasName, Type = typeof(SKElement))]
    public class DateTimePicker : Control
    {
        const string PART_CanvasName = "PART_Canvas";
        private SKElement _canvas = null!;
        static DateTimePicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DateTimePicker), new FrameworkPropertyMetadata(typeof(DateTimePicker)));
        }


        public override void OnApplyTemplate()
        {
            if (_canvas != null)
            {
                _canvas.PaintSurface -= OnPaintCanvas;
            }
            base.OnApplyTemplate();
            _canvas = (SKElement)GetTemplateChild(PART_CanvasName);
            _canvas.PaintSurface += OnPaintCanvas;
        }

        private void OnPaintCanvas(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
