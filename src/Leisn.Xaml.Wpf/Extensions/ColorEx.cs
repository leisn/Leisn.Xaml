// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Globalization;

using Leisn.Common.Media;

using SkiaSharp;

namespace System.Windows.Media
{
    public static class ColorEx
    {
        public static Color FromHex(string hex)
        {
            ArgumentNullException.ThrowIfNull(hex, nameof(hex));
            if (hex.StartsWith("#"))
            {
                hex = hex[1..];
            }

            byte a = 0xFF, r, g, b;
            if (hex.Length == 3)
            {
                string temp = hex.Substring(0, 1);
                temp += temp;
                r = byte.Parse(temp, NumberStyles.HexNumber);
                temp = hex.Substring(1, 1);
                temp += temp;
                g = byte.Parse(temp, NumberStyles.HexNumber);
                temp = hex.Substring(2, 1);
                temp += temp;
                b = byte.Parse(temp, NumberStyles.HexNumber);
            }
            else if (hex.Length == 4)
            {
                string temp = hex.Substring(0, 1);
                temp += temp;
                a = byte.Parse(temp, NumberStyles.HexNumber);
                temp = hex.Substring(1, 1);
                temp += temp;
                r = byte.Parse(temp, NumberStyles.HexNumber);
                temp = hex.Substring(2, 1);
                temp += temp;
                g = byte.Parse(temp, NumberStyles.HexNumber);
                temp = hex.Substring(3, 1);
                temp += temp;
                b = byte.Parse(temp, NumberStyles.HexNumber);
            }
            else if (hex.Length == 6)
            {
                r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            }
            else if (hex.Length == 8)
            {
                a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
                r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
            }
            else
            {
                throw new ArgumentException($"Wrong format {hex}", nameof(hex));
            }
            return Color.FromArgb(a, r, g, b);
        }
        public static Color With(this Color color, byte? A = null, byte? R = null, byte? G = null, byte? B = null)
        {
            return color with
            {
                A = A ?? color.A,
                R = R ?? color.R,
                G = G ?? color.G,
                B = B ?? color.B,
            };
        }

        public static ref Color Set(ref this Color color, string hex)
        {
            Color target = FromHex(hex);
            return ref color.Set(A: target.A, R: target.R, G: target.G, B: target.B);
        }

        public static ref Color Set(ref this Color color, byte? A = null, byte? R = null, byte? G = null, byte? B = null)
        {
            if (A is not null)
            {
                color.A = A.Value;
            }

            if (R is not null)
            {
                color.R = R.Value;
            }

            if (G is not null)
            {
                color.G = G.Value;
            }

            if (B is not null)
            {
                color.B = B.Value;
            }

            return ref color;
        }

        public static string ToHex(this Color color, bool withSymbol = true)
        {
            string hex = $"{color.R:X2}{color.G:X2}{color.B:X2}";
            if (color.A != 0xFF)
            {
                hex = $"{color.A:X2}{hex}";
            }

            if (withSymbol)
            {
                hex = $"#{hex}";
            }

            return hex;
        }

        public static Color Inverse(this Color self, bool alphaInverse = false)
        {
            int r = 255 - self.R;
            int g = 255 - self.G;
            int b = 255 - self.B;
            return Color.FromArgb(alphaInverse ? (byte)(255 - self.A) : self.A, (byte)r, (byte)g, (byte)b);
        }

        public static bool ForegroundShouldBeLight(this Rgb color)
        {
            double rg = color.R <= 10 ? color.R / 3294.0 : Math.Pow(color.R / 269.0 + 0.0513, 2.4);
            double gg = color.G <= 10 ? color.G / 3294.0 : Math.Pow(color.G / 269.0 + 0.0513, 2.4);
            double bg = color.B <= 10 ? color.B / 3294.0 : Math.Pow(color.B / 269.0 + 0.0513, 2.4);
            return 0.2126 * rg + 0.7152 * gg + 0.0722 * bg <= 0.5;
        }

        public static bool ForegroundShouldBeLight(this Hsv hsv)
        {
            return hsv.V <= 0.5 || hsv.S >= 0.5;
        }

        public static Rgb ToRgb(this Color color)
        {
            return new Rgb(color.R, color.G, color.B);
        }

        public static Hsv ToHsv(this Color color)
        {
            return color.ToRgb().ToHsv();
        }

        public static Color ToColor(this Rgb rgb)
        {
            return Color.FromRgb(rgb.R, rgb.G, rgb.B);
        }

        public static Color ToColor(this Hsv hsv)
        {
            return hsv.ToRgb().ToColor();
        }

        public static SKColor ToSKColor(this Hsv hsv)
        {
            var rgb = hsv.ToRgb();
            return new SKColor(rgb.R, rgb.G, rgb.B);
        }
    }
}
