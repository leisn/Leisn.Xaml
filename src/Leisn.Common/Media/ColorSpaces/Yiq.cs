// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Media
{
    public struct Yiq : IColor
    {
        public double Y { get; set; }
        public double I { get; set; }
        public double Q { get; set; }
    }
}
