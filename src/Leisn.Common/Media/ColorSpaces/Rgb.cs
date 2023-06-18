// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Leisn.Common.Media
{
    public struct Rgb : IColor
    {
        /// <summary>
        /// Red, 红
        /// </summary>
        public byte R { get; set; }
        /// <summary>
        /// Green, 绿
        /// </summary>
        public byte G { get; set; }
        /// <summary>
        /// Blue, 蓝
        /// </summary>
        public byte B { get; set; }

        public Rgb(byte r, byte g, byte b)
        {
            R = r; G = g; B = b;
        }

        public static Rgb FromHex(string hex)
        {
            if (hex is null) throw new ArgumentNullException(nameof(hex));
            if (hex.StartsWith("#"))
                hex = hex[1..];
            byte r, g, b;
            if (hex.Length == 3)
            {
                var temp = hex.Substring(0, 1);
                temp += temp;
                r = byte.Parse(temp, NumberStyles.HexNumber);
                temp = hex.Substring(1, 1);
                temp += temp;
                g = byte.Parse(temp, NumberStyles.HexNumber);
                temp = hex.Substring(2, 1);
                temp += temp;
                b = byte.Parse(temp, NumberStyles.HexNumber);
            }
            else if (hex.Length == 6)
            {
                r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            }
            else
            {
                throw new ArgumentException($"Wrong format {hex}", nameof(hex));
            }
            return new Rgb(r, g, b);
        }

        public string ToHex() => $"#{R:X2}{G:X2}{B:X2}";

        public override string ToString() => $"rgb({R}, {G}, {B})";

        public Hsv ToHsv()
        {
            var r = R / 255d;
            var g = G / 255d;
            var b = B / 255d;
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chrom = max - min;
            double v = max;
            double s, h = 0;
            if (chrom == 0)
            {
                s = 0;
            }
            else
            {
                s = chrom / v;
                if (max == r)
                    h = (g - b) / chrom + (g < b ? 6 : 0);
                else if (max == g)
                    h = (b - r) / chrom + 2;
                else if (max == b)
                    h = (r - g) / chrom + 4;
                h /= 6;
            }
            return new Hsv((ushort)(h * 360), s, v);
        }

        public Hsl ToHsl()
        {
            var r = R / 255d;
            var g = G / 255d;
            var b = B / 255d;
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chrom = max - min;

            double h = 0;
            double s = 0;
            double l = (max + min) / 2;
            if (chrom == 0)
            {
                l = 0;
            }
            else
            {
                s = chrom == 0 ? 0 : l > 0.5 ? chrom / (2 - 2 * l) : chrom / (2 * l);
                if (max == r)
                    h = (g - b) / chrom + (g < b ? 6 : 0);
                else if (max == g)
                    h = (b - r) / chrom + 2;
                else if (max == b)
                    h = (r - g) / chrom + 4;
                h /= 6;
            }
            return new Hsl((ushort)(360 * h), (byte)(s * 100), (byte)(l * 100));
        }

    }
}
