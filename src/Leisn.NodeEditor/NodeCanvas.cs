// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Drawing;

using Leisn.NodeEditor.Controls;

using SkiaSharp;

namespace Leisn.NodeEditor
{
    public class NodeCanvas : NodeElementBase
    {
        const float MaxScale = 5f;
        const float MinScale = .25f;
        public float ScaleStep { get; set; } = 0.25f;
        public float CellWidth { get; set; } = 10f;
        public SKColor GridAccentColor { get; set; } = SKColor.Parse("#0B0B0B");
        public SKColor GridColor { get; set; } = SKColor.Parse("#191919");
        public SKColor BackgroundColor { get; set; } = SKColor.Parse("#232323");
        public float GridStrokeWidth { get; set; } = 1f;
        public float GridAccentStrokeWidth { get; set; } = 1f;

        int times;
        public void Draw(SKSurface surface, SKImageInfo info)
        {
            Size = info.Rect.Size;
            Draw(surface.Canvas);
        }

        public override void Draw(SKCanvas canvas)
        {
            times++;
            canvas.Clear(BackgroundColor);
            canvas.SetMatrix(SKMatrix.CreateTranslation(Bounds.Left, Bounds.Top));
            DrawGrid(canvas);
            canvas.SetMatrix(LocalTransform);
            DrawSelf(canvas);

            canvas.SetMatrix(SKMatrix.CreateTranslation(Size.Width - 40, 10));
            canvas.DrawText(times.ToString(), new SKPoint(-10, 40), new SKPaint { IsAntialias = true, Color = SKColors.White });
        }
        private void DrawGrid(SKCanvas g)
        {
            var paint = new SKPaint { IsAntialias = false };

            var left = -Location.X;
            var top = -Location.Y;
            var right = Bounds.Right - Location.X;
            var bottom = Bounds.Bottom - Location.Y;
            var cellWidth = CellWidth * Scale;

            //右侧垂直线
            var start = new SKPoint(cellWidth, top);
            var end = new SKPoint(cellWidth, bottom);
            while (end.X < right)
                drawLines("right");
            //左侧垂直线
            start = new SKPoint(-cellWidth, top);
            end = new SKPoint(-cellWidth, bottom);
            while (end.X > left)
                drawLines("left");

            //下方水平线
            start = new SKPoint(left, cellWidth);
            end = new SKPoint(right, cellWidth);
            while (end.Y < bottom)
                drawLines("down");
            //上方水平线
            start = new SKPoint(left, -cellWidth);
            end = new SKPoint(right, -cellWidth);
            while (end.Y > top)
                drawLines("up");

            paint.Color = SKColors.White;
            paint.StrokeWidth = GridAccentStrokeWidth;
            if (Location.X != 0)
                g.DrawLine(0, top, 0, bottom, paint);
            if (Location.Y != 0)
                g.DrawLine(left, 0, right, 0, paint);

            void drawLines(string direction)
            {
                float xoffset = 0, yoffset = 0;
                switch (direction)
                {
                    case "left":
                        xoffset = -cellWidth;
                        break;
                    case "up":
                        yoffset = -cellWidth;
                        break;
                    case "right":
                        xoffset = cellWidth;
                        break;
                    case "down":
                        yoffset = cellWidth;
                        break;
                }

                paint.Color = GridColor;
                paint.StrokeWidth = GridStrokeWidth;
                for (int i = 0; i < 4; i++)
                {
                    g.DrawLine(start, end, paint);
                    start.X += xoffset;
                    end.X += xoffset;
                    start.Y += yoffset;
                    end.Y += yoffset;
                }
                paint.Color = GridAccentColor;
                paint.StrokeWidth = GridAccentStrokeWidth;
                g.DrawLine(start, end, paint);
                start.X += xoffset;
                end.X += xoffset;
                start.Y += yoffset;
                end.Y += yoffset;
            }
        }

        protected override void DrawSelf(SKCanvas canvas)
        {
        }
    }
}
