// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Helpers
{
    public static class ShaderHelper
    {
        public static double SmoothStep(double t1, double t2, double x)
        {
            x = Math.Clamp((x - t1) / (t2 - t1), 0, 1);
            return x * x * (3 - 2 * x);
        }
    }
}
