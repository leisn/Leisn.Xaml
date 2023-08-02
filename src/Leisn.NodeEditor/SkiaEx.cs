// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

using SkiaSharp;

namespace Leisn.NodeEditor
{
    internal static class SkiaEx
    {
        public static bool IsEmpty(this SKRect self)
        {
            return self.IsEmpty || self.Width == 0 || self.Height == 0;
        }

        #region ref 
        public static ref SKRect Set(ref this SKRect self, float left, float top, float right, float bottom)
        {
            self.Left = left;
            self.Top = top;
            self.Right = right;
            self.Bottom = bottom;
            return ref self;
        }

        public static ref SKRect Set(ref this SKRect self, SKPoint leftTop, SKPoint rightBottom)
        {
            return ref Set(ref self, leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);
        }

        public static ref SKRect Set(ref this SKRect self, SKPoint leftTop)
        {
            return ref self.Set(leftTop.X, leftTop.Y, self.Width, self.Height);
        }

        public static ref SKRect Set(ref this SKRect self, SKSize size)
        {
            return ref self.Set(self.Left, self.Top, size.Width + self.Left, size.Height + self.Top);
        }

        #endregion

        public static SKRect With(this SKRect self, float? Left = null, float? Top = null, float? Right = null, float? Bottom = null)
        {
            return new(Left ?? self.Left, Top ?? self.Top, Right ?? self.Right, Bottom ?? self.Bottom);
        }

        public static SKRect With(this SKRect self, SKPoint leftTop, SKPoint rightBottom)
        {
            return self.With(leftTop.X, leftTop.Y, rightBottom.X, rightBottom.Y);
        }

        public static SKRect With(this SKRect self, SKPoint location, SKSize size)
        {
            return self.With(location.X, location.Y, location.X + size.Width, location.Y + size.Height);
        }

        /// <summary>
        /// With location, keep size.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="location">New location</param>
        /// <returns>New instance</returns>
        public static SKRect With(this SKRect self, SKPoint location)
        {
            return self.With(location.X, location.Y, location.X + self.Width, location.Y + self.Height);
        }
        /// <summary>
        /// With size, keep location
        /// </summary>
        /// <param name="self"></param>
        /// <param name="size">New size</param>
        /// <returns>New instance</returns>
        public static SKRect With(this SKRect self, SKSize size)
        {
            return self.With(self.Left, self.Top, self.Left + size.Width, self.Top + size.Height);
        }
    }
}
