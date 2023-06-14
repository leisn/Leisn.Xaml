// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Media
{
    public struct Yuv : IColor
    {
        public int Y { get; set; }
        public int U { get; set; }
        public int V { get; set; }
    }
}
