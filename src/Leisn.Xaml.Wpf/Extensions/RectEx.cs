// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Collections.Generic;

using Leisn.Xaml.Wpf.Extensions;

namespace System.Windows
{
    public static class RectEx
    {
        public static Rect Create(double left, double top, double right, double bottom)
            => new(left, top, right - left, bottom - top);

        public static bool IsEmpty(this Rect self)
        {
            return self.IsEmpty || self.Width == 0 || self.Height == 0;
        }

        public static ref Rect Set(ref this Rect self, double? Left = null, double? Top = null, double? Width = null, double? Height = null)
        {
            if (Left is not null)
            {
                self.X = Left.Value;
            }
            if (Top is not null)
            {
                self.Y = Top.Value;
            }
            if (Width is not null)
            {
                self.Width = Width.Value;
            }
            if (Height is not null)
            {
                self.Height = Height.Value;
            }
            return ref self;
        }
        public static ref Rect Set(ref this Rect self, Point location)
        {
            return ref self.Set(location.X, location.Y);
        }

        public static ref Rect Set(ref this Rect self, Size size)
        {
            return ref self.Set(Width: size.Width, Height: size.Height);
        }

        public static ref Rect SetBounds(ref this Rect self, double? Left = null, double? Top = null, double? Right = null, double? Bottom = null)
        {
            if (Left is not null)
            {
                self.X = Left.Value;
            }
            if (Top is not null)
            {
                self.Y = Top.Value;
            }
            if (Right is not null)
            {
                self.Width = Right.Value - self.X;
            }
            if (Bottom is not null)
            {
                self.Height = Bottom.Value - self.Y;
            }
            return ref self;
        }
        public static ref Rect SetBounds(ref this Rect self, Point rightBottom)
        {
            self.Width = rightBottom.X - self.X;
            self.Height = rightBottom.Y - self.Y;
            return ref self;
        }


        public static Rect With(this Rect self, double? Left = null, double? Top = null, double? Width = null, double? Height = null)
        {
            return new(Left ?? self.Left, Top ?? self.Top, Width ?? self.Width, Height ?? self.Height);
        }

        public static Rect With(this Rect self, Point location)
        {
            return self.With(location.X, location.Y);
        }

        public static Rect With(this Rect self, Size size)
        {
            return self.With(Width: size.Width, Height: size.Height);
        }

        public static Rect WithBounds(this Rect self, double? Left = null, double? Top = null, double? Right = null, double? Bottom = null)
        {
            return new(Left ?? self.X, Top ?? self.Y, Right == null ? self.Width : Right.Value - self.X, Bottom == null ? self.Height : Bottom.Value - self.Y);
        }

        public static Rect WithBounds(this Rect self, Point rightBottom)
        {
            return new(self.X, self.Y, rightBottom.X - self.X, rightBottom.Y - self.Y);
        }

        public static Rect GetOutBounds(this IEnumerable<Rect> rects)
        {
            double left = double.PositiveInfinity;
            double top = double.PositiveInfinity;
            double right = 0;
            double bottom = 0;
            foreach (Rect item in rects)
            {
                left = Math.Min(item.Left, left);
                top = Math.Min(item.Top, top);
                right = Math.Max(item.Right, right);
                bottom = Math.Max(item.Bottom, bottom);
            }
            if (right == 0 || bottom == 0 || left == double.PositiveInfinity || top == double.PositiveInfinity)
            {
                return default;
            }

            return new Rect(left, top, right - left, bottom - top);
        }

        public static bool TryMerge(this Rect self, Rect rect, out Rect Target)
        {
            bool merged = false;
            if (self.Contains(rect))
            {
                Target = self;
                merged = true;
            }
            else if (rect.Contains(self))
            {
                Target = rect;
                merged = true;
            }
            else if (self.IntersectsWith(rect))
            {
                if (self.Top == rect.Top && self.Bottom == rect.Bottom)
                {
                    double left = Math.Min(self.Left, rect.Left);
                    double right = Math.Max(self.Right, rect.Right);
                    Target = new Rect(left, self.Top, right - left, self.Height);
                    merged = true;
                }
                else if (self.Left == rect.Left && self.Right == rect.Right)
                {
                    double top = Math.Min(self.Top, rect.Top);
                    double bottom = Math.Max(self.Bottom, rect.Bottom);
                    Target = new Rect(self.Left, top, self.Width, bottom - top);
                    merged = true;
                }
            }
            return merged;
        }

        /// <summary>
        /// 返回左上角重叠区域，及分割后右方和下方区域
        /// </summary>
        public static (Rect Target, Rect? ClipRight, Rect? ClipBottom) Clip(this Rect self, Size size)
        {
            double width = Math.Min(self.Width, size.Width);
            double height = Math.Min(self.Height, size.Height);
            Rect target = new(self.Left, self.Top, size.Width, size.Height);

            double rightSpace = self.Width - width,
                   bottomSpace = self.Height - height;

            Rect? clipRight = null;
            if (rightSpace > 0)
            {
                clipRight = new Rect(target.Right, self.Top, rightSpace, self.Height);
            }

            Rect? clipBottom = null;
            if (bottomSpace > 0)
            {
                clipBottom = new Rect(self.Left, target.Bottom, self.Width, bottomSpace);
            }

            return (target, clipRight, clipBottom);
        }

        /// <summary>
        /// 返回分割后的区域
        /// </summary>
        public static (bool Clipped, RectClip ClipResult) Clip(this Rect self, Rect rect)
        {
            (bool clipped, Rect target) = self.ClipOverlap(rect);
            if (!clipped)
            {
                return (false, default);
            }

            double leftSpace = target.Left - self.Left,
                rightSpace = self.Right - target.Right,
                topSpace = target.Top - self.Top,
                bottomSpace = self.Bottom - target.Bottom;

            RectClip result = new() { Target = target };
            if (topSpace > 0)
            {
                result.Top = new Rect(self.Left, self.Top, self.Width, topSpace);
            }

            if (rightSpace > 0)
            {
                result.Right = new Rect(target.Right, self.Top, rightSpace, self.Height);
            }

            if (leftSpace > 0)
            {
                result.Left = new Rect(self.Left, self.Top, leftSpace, self.Height);
            }

            if (bottomSpace > 0)
            {
                result.Bottom = new Rect(self.Left, target.Bottom, self.Width, bottomSpace);
            }

            return (true, result);
        }

        /// <summary>
        /// 返回重叠区域
        /// </summary>
        public static (bool Overlap, Rect Clip) ClipOverlap(this Rect self, Rect rect)
        {
            double left = rect.Left - self.Left < 0 ? self.Left : rect.Left;
            double top = rect.Top - self.Top < 0 ? self.Top : rect.Top;
            double right = rect.Right - self.Right < 0 ? rect.Right : self.Right;
            double bottom = rect.Bottom - self.Bottom < 0 ? rect.Bottom : self.Bottom;

            double width = right - left;
            if (width < 0)
            {
                width = 0;
            }

            double height = bottom - top;
            if (height < 0)
            {
                height = 0;
            }

            return (right > left && bottom > top, new Rect(left, top, width, height));
        }
    }
}
