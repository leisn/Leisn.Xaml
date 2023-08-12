// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;

namespace Leisn.NodeEditor.Framework
{
    [Flags]
    public enum MouseButton
    {
        None,
        Left = 1,
        Middle = 2,
        Right = 4
    }

    public class MouseEvent
    {
        public MouseButton Button { get; }
        public SKPoint Position { get; }
        public bool NeedRedraw { get; set; }
        public bool Handled { get; set; }

        public bool IsCtrlDown { get; set; }
        public bool IsShiftDown { get; set; }
        public bool IsAltDown { get; set; }

        public int Timestamp { get; set; }

        public MouseEvent(MouseButton button, SKPoint position, int timestamp, bool isCtrlDown, bool isShiftDown, bool isAltDown)
        {
            Button = button;
            Position = position;
            IsCtrlDown = isCtrlDown;
            IsShiftDown = isShiftDown;
            IsAltDown = isAltDown;
            Timestamp = timestamp;
        }
    }

    public class MouseWheelEvent : MouseEvent
    {
        public int Delta { get; }
        public MouseWheelEvent(int delta, SKPoint position, int timestamp, bool isCtrlDown, bool isShiftDown, bool isAltDown)
            : base(MouseButton.Middle, position, timestamp, isCtrlDown, isShiftDown, isAltDown)
        {
            Delta = delta;
        }
    }
}
