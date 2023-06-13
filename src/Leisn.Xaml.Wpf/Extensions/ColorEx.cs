// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Globalization;

namespace System.Windows.Media
{
    public static class ColorEx
    {
        public static Color FromHex(string hex)
        {
            ArgumentNullException.ThrowIfNull(hex, nameof(hex));
            if (hex.StartsWith("#"))
                hex = hex[1..];
            byte a = 0xFF, r, g, b;
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
            else if (hex.Length == 4)
            {
                var temp = hex.Substring(0, 1);
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
            var target = FromHex(hex);
            return ref color.Set(A: target.A, R: target.R, G: target.G, B: target.B);
        }

        public static ref Color Set(ref this Color color, byte? A = null, byte? R = null, byte? G = null, byte? B = null)
        {
            if (A is not null)
                color.A = A.Value;
            if (R is not null)
                color.R = R.Value;
            if (G is not null)
                color.G = G.Value;
            if (B is not null)
                color.B = B.Value;
            return ref color;
        }


        public static string ToHex(this Color color, bool withSymbol = true)
        {
            var hex = $"{color.R:X2}{color.G:X2}{color.B:X2}";
            if (color.A != 0xFF)
                hex = $"{color.A:X2}{hex}";
            if (withSymbol)
                hex = $"#{hex}";
            return hex;
        }

        public static Color Inverse(this Color self, bool alphaInverse = false)
        {
            var r = 255 - self.R;
            var g = 255 - self.G;
            var b = 255 - self.B;
            return Color.FromArgb(alphaInverse ? (byte)(255 - self.A) : self.A, (byte)r, (byte)g, (byte)b);
        }
    }
}
