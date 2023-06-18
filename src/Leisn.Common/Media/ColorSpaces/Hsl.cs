// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Media
{
    public struct Hsl : IColor
    {
        /// <summary>
        /// Hue, 色相、色调, 0-360
        /// </summary>
        public ushort H { get; set; }
        /// <summary>
        /// Saturation, 饱和度, 0-100
        /// </summary>
        public byte S { get; set; }
        /// <summary>
        /// Lightness, 亮度, 0-100
        /// </summary>
        public byte L { get; set; }

        public Hsl(ushort h, byte s, byte l)
        {
            H = h; S = s; L = l;
        }

        public override string ToString() => $"hsl({H}, {S}, {L})";

        public Hsv ToHsv()
        {
            var s = S / 100d;
            var l = L / 100d;
            double v = 0;

            if (s == 0)
            {
                v = l;
            }
            else if (l > .5)
            {
                v = l + s * (1 - l);
                s = v == 0 ? 0 : 2 * s * (1 - l) / v;
            }
            else
            {
                v = l * (s + 1);
                s = v == 0 ? 0 : 2 * s / (s + 1);
            }
            return new Hsv(H, s, v);
        }

        public Rgb ToRgb()
        {
            var h = H / 360d;
            var s = S / 100d;
            var l = L / 100d;

            var r = l;
            var g = l;
            var b = l;
            if (s != 0)
            {
                var q = l < .5 ? l * (1 + s) : 1 + s - l * s;
                var p = 2 * l - q;
                double _hue2Rgb(double t)
                {
                    if (t < 0) t += 1;
                    if (t > 1) t -= 1;
                    if (t < 1 / 6)
                        return p + (q - p) * 6 * t;
                    if (t < 1 / 2)
                        return q;
                    if (t < 2 / 3)
                        return p + (q - p) * 6 * (2 / 3 - t);
                    return p;
                }
                r = _hue2Rgb(h + 1 / 3);
                g = _hue2Rgb(h);
                b = _hue2Rgb(h - 1 / 3);
            }
            return new Rgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
    }
}
