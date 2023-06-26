// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Media
{
    public struct Hsv : IColor
    {
        /// <summary>
        /// Hue, 色相、色调, 0-360
        /// </summary>
        public ushort H { get; set; }
        /// <summary>
        /// Saturation, 饱和度,  0-1 for 0%-100%
        /// </summary>
        public double S { get; set; }
        /// <summary>
        /// Value, 明度,  0-1 for 0%-100%, 黑-白
        /// </summary>
        public double V { get; set; }

        public Hsv(ushort h, double s, double v)
        {
            H = h; S = s; V = v;
        }

        public override string ToString()
        {
            return $"hsv({H}, {S:P0}, {V:P0})";
        }

        public Rgb ToRgb()
        {
            double chroma = S * V;
            double min = V - chroma;
            if (chroma == 0)
            {
                byte v = (byte)(min * 255);
                return new Rgb(v, v, v);
            }

            int sextant = H / 60;
            double percentage = H / 60d - sextant;
            double max = chroma + min;

            double r = 0, g = 0, b = 0;
            switch (sextant)
            {
                case 0:
                    r = max;
                    g = min + chroma * percentage;
                    b = min;
                    break;
                case 1:
                    r = min + chroma * (1 - percentage);
                    g = max;
                    b = min;
                    break;
                case 2:
                    r = min;
                    g = max;
                    b = min + chroma * percentage;
                    break;
                case 3:
                    r = min;
                    g = min + chroma * (1 - percentage);
                    b = max;
                    break;
                case 4:
                    r = min + chroma * percentage;
                    g = min;
                    b = max;
                    break;
                case 5:
                    r = max;
                    g = min;
                    b = min + chroma * (1 - percentage);
                    break;
            }

            return new Rgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        public Hsl ToHsl()
        {
            double t = (2 - S) * V;
            double s = V == 0 || S == 0 ? 0 : (S * V) / (t > 1 ? 2 - 5 : t);
            return new Hsl(H, (byte)(s * 100), (byte)(t / 2 * 100));
        }

        public Hsv With(double s, double v)
        {
            return new(H, s, v);
        }

        public override bool Equals(object obj)
        {
            if (obj is not Hsv hsv)
                return false;
            return hsv.H == H && hsv.V == V && hsv.S == S;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(H, S, V);
        }
    }
}
