// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Leisn.NodeEditor;
using Leisn.NodeEditor.Framework;
using Leisn.Xaml.Wpf.Extensions;

using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_CanvasName, Type = typeof(SKElement))]
    public class SkNodeEditor : Control
    {
        const string PART_CanvasName = "PART_Canvas";
        private SKElement _canvas = null!;
        private readonly NodeCanvas _nodeCanvas;

        private DpiScale _dpiScale;

        public SkNodeEditor()
        {
            _nodeCanvas = new NodeCanvas();
            _dpiScale = VisualEx.GetDpiScale();
        }
        public override void OnApplyTemplate()
        {
            if (_canvas != null)
            {
                _canvas.PaintSurface -= OnPaintSurface;
            }
            base.OnApplyTemplate();
            _canvas = (SKElement)GetTemplateChild(PART_CanvasName);
            _canvas.PaintSurface += OnPaintSurface;
        }

        private void OnPaintSurface(object? sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            _nodeCanvas?.Draw(e.Surface, e.Info);
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            _dpiScale = newDpi;
            _canvas.InvalidateVisual();
        }

        #region mouse actions
        private Point GetRealPoint(Point point)
        {
            return new(point.X * _dpiScale.DpiScaleX, point.Y * _dpiScale.DpiScaleY);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            var evt = ConvertEvent(e);
            _nodeCanvas.OnMouseDown(evt);
            if (evt.NeedRedraw)
                _canvas.InvalidateVisual();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var evt = ConvertEvent(e);
            _nodeCanvas.OnMouseMove(evt);
            if (evt.NeedRedraw)
                _canvas.InvalidateVisual();
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            var evt = ConvertEvent(e);
            _nodeCanvas.OnMouseUp(evt);
            if (evt.NeedRedraw)
                _canvas.InvalidateVisual();
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            var evt = new MouseWheelEvent(e.Delta,
                GetRealPoint(e.GetPosition(this)).ToSKPoint(), e.Timestamp,
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
            _nodeCanvas.OnMouseWheel(evt);
            if (evt.NeedRedraw)
                _canvas.InvalidateVisual();
        }

        private MouseEvent ConvertEvent(MouseEventArgs e)
        {
            var button = Leisn.NodeEditor.Framework.MouseButton.None;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                button |= Leisn.NodeEditor.Framework.MouseButton.Left;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                button |= Leisn.NodeEditor.Framework.MouseButton.Right;
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                button |= Leisn.NodeEditor.Framework.MouseButton.Middle;
            }
            var evt = new MouseEvent(button,
                GetRealPoint(e.GetPosition(this)).ToSKPoint(), e.Timestamp,
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
            return evt;
        }
        #endregion

    }
}
