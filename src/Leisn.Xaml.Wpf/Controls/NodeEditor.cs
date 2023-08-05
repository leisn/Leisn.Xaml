// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Leisn.NodeEditor;
using Leisn.NodeEditor.Events;
using Leisn.Xaml.Wpf.Extensions;

using SkiaSharp.Views.WPF;

namespace Leisn.Xaml.Wpf.Controls
{
    [TemplatePart(Name = PART_CanvasName, Type = typeof(SKElement))]
    public class NodeEditor : Control
    {
        const string PART_CanvasName = "PART_Canvas";
        private SKElement _canvas = null!;
        private readonly NodeCanvas _nodeCanvas;

        private DpiScale _dpiScale;

        public NodeEditor()
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
            var evt = new MouseEvent(GetButton(e), GetRealPoint(e.GetPosition(this)).ToSKPoint());
            _nodeCanvas.OnMouseDown(evt);
            if (evt.NeedRedraw)
                _canvas.InvalidateVisual();
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            var evt = new MouseEvent(GetButton(e), GetRealPoint(e.GetPosition(this)).ToSKPoint());
            _nodeCanvas.OnMouseMove(evt);
            if (evt.NeedRedraw)
                _canvas.InvalidateVisual();
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            var evt = new MouseEvent(GetButton(e), GetRealPoint(e.GetPosition(this)).ToSKPoint());
            _nodeCanvas.OnMouseUp(evt);
            if (evt.NeedRedraw)
                _canvas.InvalidateVisual();
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            _nodeCanvas.OnMouseWheel(new MouseWheelEvent(e.Delta, GetRealPoint(e.GetPosition(this)).ToSKPoint()));
            _canvas.InvalidateVisual();
        }

        private static Leisn.NodeEditor.Events.MouseButton GetButton(MouseEventArgs e)
        {
            var button = Leisn.NodeEditor.Events.MouseButton.None;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                button |= Leisn.NodeEditor.Events.MouseButton.Left;
            }
            if (e.RightButton == MouseButtonState.Pressed)
            {
                button |= Leisn.NodeEditor.Events.MouseButton.Right;
            }
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                button |= Leisn.NodeEditor.Events.MouseButton.Middle;
            }
            return button;
        }
        #endregion

        #region keyboard actions
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        #endregion
    }
}
