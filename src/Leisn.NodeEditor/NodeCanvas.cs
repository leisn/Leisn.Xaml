// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Drawing;

using Leisn.NodeEditor.Controls;

using SkiaSharp;

namespace Leisn.NodeEditor
{
    public class NodeCanvas
    {
        const float MaxScale = 5f;
        const float MinScale = .25f;
        public float Scale = 1;
        public float ScaleStep { get; set; } = 0.25f;
        public float CellWidth { get; set; } = 10f;
        public SKColor GridAccentColor { get; set; } = SKColor.Parse("#0B0B0B");
        public SKColor GridColor { get; set; } = SKColor.Parse("#191919");
        public SKColor BackgroundColor { get; set; } = SKColor.Parse("#232323");
        public float GridStrokeWidth { get; set; } = 1f;
        public float GridAccentStrokeWidth { get; set; } = 1f;

        int times;

        SKPoint _location = new(-30, -20);
        SKSize _size;

        public void Draw(SKSurface surface, SKImageInfo info)
        {
            _size = info.Size;
            var canvas = surface.Canvas;
            canvas.Clear(BackgroundColor);
            DrawLines(canvas);
            canvas.SetMatrix(SKMatrix.CreateScale(Scale, Scale).PostConcat(SKMatrix.CreateTranslation(_location.X, _location.Y)));
            //draw children
            canvas.SetMatrix(SKMatrix.CreateTranslation(_size.Width - 20, 10));
            canvas.DrawText(times.ToString(), new SKPoint(-10, 20), new SKPaint { IsAntialias = true, Color = SKColors.White });
        }

        private void DrawLines(SKCanvas canvas)
        {
            using var paint = new SKPaint { IsAntialias = false, Color = GridColor, StrokeWidth = GridStrokeWidth };

            var cellWidth = CellWidth * Scale;
            var blockWidth = cellWidth * 5;
            var left = -_location.X;
            var top = -_location.Y;
            var right = _size.Width;
            var bottom = _size.Height;

            while (top <= bottom)//水平线
            {
                paint.Color = GridAccentColor;
                paint.StrokeWidth = GridAccentStrokeWidth;
                canvas.DrawLine(0, top, right, top, paint);
                paint.Color = GridColor;
                paint.StrokeWidth = GridStrokeWidth;
                for (int i = 1; i < 5; i++)
                {
                    top += cellWidth;
                    canvas.DrawLine(0, top, right, top, paint);
                }
            }

            while (left <= right) //垂直线
            {
                paint.Color = GridAccentColor;
                paint.StrokeWidth = GridAccentStrokeWidth;
                canvas.DrawLine(left, 0, left, bottom, paint);
                paint.Color = GridColor;
                paint.StrokeWidth = GridStrokeWidth;
                for (int i = 1; i < 5; i++)
                {
                    left += cellWidth;
                    canvas.DrawLine(left, 0, left, bottom, paint);
                }
            }

            //paint.Color = SKColors.White;
            //paint.StrokeWidth = GridAccentStrokeWidth;
            //if (Bounds.Left < 0 && Bounds.Right > 0)
            //    canvas.DrawLine(Bounds.Left, Bounds.Top, Bounds.Left, Bounds.Bottom, paint);
            //if (Bounds.Top < 0 && Bounds.Bottom > 0)
            //    canvas.DrawLine(Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Top, paint);
        }

    }
}
