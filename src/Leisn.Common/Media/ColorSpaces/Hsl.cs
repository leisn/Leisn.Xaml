// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Media
{
    public struct Hsl : IColor
    {
        /// <summary>
        /// Hue, 色相、色调
        /// </summary>
        public ushort H { get; set; }
        /// <summary>
        /// Saturation, 饱和度
        /// </summary>
        public byte S { get; set; }
        /// <summary>
        /// Lightness, 亮度
        /// </summary>
        public byte L { get; set; }

        public Hsl(ushort h, byte s, byte l)
        {
            H = h; S = s; L = l;
        }
    }
}
