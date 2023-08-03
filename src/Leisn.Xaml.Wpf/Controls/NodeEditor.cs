// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Leisn.NodeEditor;

using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_CanvasName, Type = typeof(SKElement))]
    public class NodeEditor : Control
    {
        const string PART_CanvasName = "PART_Canvas";
        private SKElement _canvas = null!;
        private NodeCanvas? _nodeCanvas;

        public override void OnApplyTemplate()
        {
            if (_canvas != null)
            {
                _canvas.PaintSurface -= OnPaintSurface;
            }
            base.OnApplyTemplate();
            _canvas = (SKElement)GetTemplateChild(PART_CanvasName);
            _canvas.PaintSurface += OnPaintSurface;
            _nodeCanvas ??= new NodeCanvas();
        }

        private void OnPaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            _nodeCanvas?.Draw(e.Surface, e.Info);
        }

    }
}
