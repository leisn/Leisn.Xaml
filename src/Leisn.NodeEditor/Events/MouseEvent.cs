// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

using SkiaSharp;

namespace Leisn.NodeEditor.Events
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

        public MouseEvent(MouseButton button, SKPoint position)
        {
            Button = button;
            Position = position;
        }
    }

    public class MouseWheelEvent
    {
        public int Delta { get; }
        public SKPoint Position { get; }
        public MouseWheelEvent(int delta, SKPoint position)
        {
            Delta = delta;
            Position = position;
        }
    }
}
