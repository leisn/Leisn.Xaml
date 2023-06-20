// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkiaSharp;

namespace Leisn.Xaml.Wpf.Extensions
{
    public static class SkiaEx
    {
        public static SKRect OffsetToCenter(this SKRect rect, float left, float top, float right, float bottom)
            => new(rect.Left + left, rect.Top + top, rect.Right - right, rect.Bottom - bottom);
        public static SKRect OffsetToCenter(this SKRect rect, float h, float v)
            => rect.OffsetToCenter(h, v, h, v);
        public static SKRect OffsetToCenter(this SKRect rect, float offset)
            => rect.OffsetToCenter(offset, offset);

        public static SKRect CreateRect(float centerX, float centerY, float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
            => new(centerX - offsetLeft, centerY - offsetTop, centerX + offsetRight, centerY + offsetBottom);
        public static SKRect CreateRect(float centerX, float centerY, float offsetH, float offsetV)
            => CreateRect(centerX, centerY, offsetH, offsetV, offsetH, offsetV);
        public static SKRect CreateRect(float centerX, float centerY, float offset)
            => CreateRect(centerX, centerY, offset, offset);
        public static SKRect CreateRect(SKPoint center, float offset)
            => CreateRect(center.X, center.Y, offset, offset);
    }
}
