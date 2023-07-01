// @Leisn (https://leisn.com , https://github.com/leisn)

namespace SkiaSharp
{
    public static class SkiaEx
    {
        public static SKRect OffsetToCenter(this SKRect rect, float left, float top, float right, float bottom)
        {
            return new(rect.Left + left, rect.Top + top, rect.Right - right, rect.Bottom - bottom);
        }

        public static SKRect OffsetToCenter(this SKRect rect, float h, float v)
        {
            return rect.OffsetToCenter(h, v, h, v);
        }

        public static SKRect OffsetToCenter(this SKRect rect, float offset)
        {
            return rect.OffsetToCenter(offset, offset);
        }

        public static SKRect CreateRect(float centerX, float centerY, float offsetLeft, float offsetTop, float offsetRight, float offsetBottom)
        {
            return new(centerX - offsetLeft, centerY - offsetTop, centerX + offsetRight, centerY + offsetBottom);
        }

        public static SKRect CreateRect(float centerX, float centerY, float offsetH, float offsetV)
        {
            return CreateRect(centerX, centerY, offsetH, offsetV, offsetH, offsetV);
        }

        public static SKRect CreateRect(float centerX, float centerY, float offset)
        {
            return CreateRect(centerX, centerY, offset, offset);
        }

        public static SKRect CreateRect(SKPoint center, float offset)
        {
            return CreateRect(center.X, center.Y, offset, offset);
        }
    }
}
