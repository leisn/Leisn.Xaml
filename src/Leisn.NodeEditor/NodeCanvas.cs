﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Input;

using Leisn.NodeEditor.Controls;
using Leisn.NodeEditor.Framework;

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
            times++;
            _size = info.Size;
            var canvas = surface.Canvas;
            canvas.Clear(BackgroundColor);
            var tanslation = SKMatrix.CreateTranslation(_location.X, _location.Y);
            DrawLines(canvas);
            canvas.SetMatrix(SKMatrix.CreateScale(Scale, Scale).PostConcat(tanslation));
            //draw children
            canvas.SetMatrix(SKMatrix.CreateTranslation(_size.Width - 20, 10));
            canvas.DrawText(times.ToString(), new SKPoint(-10, 10), new SKPaint { IsAntialias = true, Color = SKColors.White });
            canvas.DrawText($"({Math.Round(_location.X, 2)},{Math.Round(_location.Y, 2)})", new SKPoint(-40, 30), new SKPaint { IsAntialias = true, Color = SKColors.White });
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
            float x = left, y = top;

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
                canvas.DrawLine(x, 0, x, bottom, paint);
            if (_location.Y > 0)
                canvas.DrawLine(0, y, right, y, paint);
        }

        #region mouse event
        private SKPoint? _pressPoint;
        private SKPoint _orginPoint;
        public void OnMouseDown(MouseEvent mouseEvent)
        {
            _pressPoint = mouseEvent.Position;
            _orginPoint = _location;
        }
        public void OnMouseMove(MouseEvent mouseEvent)
        {
            if (_pressPoint != null)
            {
                var offset = mouseEvent.Position - _pressPoint.Value;
                _location = offset + _orginPoint;
                mouseEvent.NeedRedraw = true;
            }
        }

        public void OnMouseUp(MouseEvent mouseEvent)
        {
            _pressPoint = null;
        }

        public void OnMouseWheel(MouseWheelEvent mouseWheelEvent)
        {
            var oldScale = Scale;
            if (mouseWheelEvent.Delta > 0)
                Scale = Math.Min(MaxScale, Scale + ScaleStep);
            else
                Scale = Math.Max(MinScale, Scale - ScaleStep);
            if (oldScale == Scale)
                return;
            var posToSelf = mouseWheelEvent.Position - _location;
            var scale = Scale / oldScale;
            var posScaled = new SKPoint(scale * posToSelf.X, scale * posToSelf.Y);
            var scaledOffset = posScaled - posToSelf;
            _location -= scaledOffset;
            mouseWheelEvent.Handled = true;
            mouseWheelEvent.NeedRedraw = true;
        }
        #endregion
    }
}
