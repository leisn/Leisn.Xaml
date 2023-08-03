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

        SKPoint _location = new(60, 60);
        SKSize _size;

        public void Draw(SKSurface surface, SKImageInfo info)
        {
            _size = info.Size;
            var canvas = surface.Canvas;
            canvas.Clear(BackgroundColor);
            var tanslation = SKMatrix.CreateTranslation(_location.X, _location.Y);
            DrawLines(canvas);
            canvas.SetMatrix(SKMatrix.CreateScale(Scale, Scale).PostConcat(tanslation));
            //draw children
            canvas.SetMatrix(SKMatrix.CreateTranslation(_size.Width - 20, 10));
            canvas.DrawText(times.ToString(), new SKPoint(-10, 10), new SKPaint { IsAntialias = true, Color = SKColors.White });
            canvas.DrawText($"({_location.X},{_location.Y})", new SKPoint(-40, 30), new SKPaint { IsAntialias = true, Color = SKColors.White });
        }

        private void DrawLines(SKCanvas canvas)
        {
            using var paint = new SKPaint { IsAntialias = false, Color = GridColor, StrokeWidth = GridStrokeWidth };
            var cellWidth = CellWidth * Scale;
            var blockWidth = cellWidth * 5;
            var right = _size.Width;
            var bottom = _size.Height;
            var left = _location.X;
            var top = _location.Y;

            left = left <= 0 ? left % blockWidth : left % blockWidth - blockWidth;
            top = top <= 0 ? top % blockWidth : top % blockWidth - blockWidth;
            //if (left > 0) while (left >= 0) left -= blockWidth;
            //else if (left < 0) while (left + blockWidth <= 0) left += blockWidth;
            //if (top > 0) while (top >= 0) top -= blockWidth;
            //else if (top < 0) while (top + blockWidth <= 0) top += blockWidth;

            while (top <= bottom)//水平线
            {
                paint.Color = GridAccentColor;
                paint.StrokeWidth = GridAccentStrokeWidth;
                canvas.DrawLine(0, top, right, top, paint);
                paint.Color = GridColor;
                paint.StrokeWidth = GridStrokeWidth;
                top += cellWidth;
                for (int i = 1; i < 5; i++)
                {
                    canvas.DrawLine(0, top, right, top, paint);
                    top += cellWidth;
                }
            }

            while (left <= right) //垂直线
            {
                paint.Color = GridAccentColor;
                paint.StrokeWidth = GridAccentStrokeWidth;
                canvas.DrawLine(left, 0, left, bottom, paint);
                paint.Color = GridColor;
                paint.StrokeWidth = GridStrokeWidth;
                left += cellWidth;
                for (int i = 1; i < 5; i++)
                {
                    canvas.DrawLine(left, 0, left, bottom, paint);
                    left += cellWidth;
                }
            }

            paint.Color = SKColors.White;
            paint.StrokeWidth = GridAccentStrokeWidth;
            if (_location.X > 0)
                canvas.DrawLine(_location.X, 0, _location.X, bottom, paint);
            if (_location.Y > 0)
                canvas.DrawLine(0, _location.Y, right, _location.Y, paint);
        }

    }
}
