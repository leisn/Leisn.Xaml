﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

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
        /// Value, 明度,  0-1 for 0%-100%
        /// </summary>
        public double V { get; set; }

        public Hsv(ushort h, double s, double v)
        {
            H = h; S = s; V = v;
        }

        public override string ToString() => $"hsv({H}, {S:P0}, {V:P0})";

        public Rgb ToRgb()
        {
            var chroma = S * V;
            var min = V - chroma;
            if (chroma == 0)
            {
                var v = (byte)(min * 255);
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
    }
}