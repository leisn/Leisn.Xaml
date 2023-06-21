// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Media
{
    public static class ShapeHelper
    {
        /// <summary>
        /// 圆形到正方形点位映射
        /// </summary>
        /// <param name="u">x位置,[-1,1]</param>
        /// <param name="v">y位置,[-1,1]</param>
        /// <returns>(X,Y)，值范围为-1 到 1</returns>
        public static (double X, double Y) DiscToSquareMapping(double u, double v)
        {
            double u2 = u * u;
            double v2 = v * v;
            double twosqrt2 = 2 * Math.Sqrt(2);
            double subtermx = 2 + u2 - v2;
            double subtermy = 2 - u2 + v2;
            double termx1 = subtermx + u * twosqrt2;
            double termx2 = subtermx - u * twosqrt2;
            double termy1 = subtermy + v * twosqrt2;
            double termy2 = subtermy - v * twosqrt2;
            double x = 0.5 * Math.Sqrt(termx1) - 0.5 * Math.Sqrt(termx2);
            double y = 0.5 * Math.Sqrt(termy1) - 0.5 * Math.Sqrt(termy2);
            return (x, y);
        }

        /// <summary>
        /// 长方形到圆形点位映射
        /// </summary>
        /// <param name="x">x位置, [-1,1]</param>
        /// <param name="y">y位置, [-1,1]</param>
        /// <returns>(U,V), 即(X,Y), 值范围为-1 到 1</returns>
        public static (double U, double V) SquareToDiscMapping(double x, double y)
        {
            double u = x * Math.Sqrt(1 - y * y / 2);
            double v = y * Math.Sqrt(1 - x * x / 2);
            return (u, v);
        }


        /// <summary>
        /// 笛卡尔直角坐标系(Y轴上方正向)转极坐标系
        /// </summary>
        /// <returns>
        /// (距离，弧度[-π≤θ≤π]，相对X轴)<br/>
        /// <code>
        ///       2(π/2,π]  | 1(0,π/2)                         
        /// 返回 ---------------------------
        ///       3(-π,-π/2)| 4(-π/2,0)                        
        /// </code>
        /// </returns>
        public static (double Distance, double Radians) CartesianToPolar(double x, double y)
        {
            double distance = Math.Sqrt(x * x + y * y);
            double radians = Math.Atan2(y, x);
            return (distance, radians);
        }

        /// <summary>
        /// 极坐标系转笛卡尔直角坐标系(Y轴上方正向)
        /// </summary>
        public static (double X, double Y) PolarToCartesian(double distance, double radians)
        {
            double x = distance * Math.Cos(radians);
            double y = distance * Math.Sin(radians);
            return (x, y);
        }

        /// <summary>
        /// 获取点位所在象限(Y轴上方正向)
        /// <code>
        ///       2(-,+)|1(+,+)                     3(-,-)|4(+,-)
        /// 返回 ----------------  对于左上角(0, 0) ---------------
        ///       3(-,-)|4(+,-)                     2(-,+)|1(+,+)
        /// </code>
        /// </summary>
        public static int Quadrant(double pointX, double pointY, double centerX = 0, double centerY = 0)
        {
            double offsetX = pointX - centerX;
            double offsetY = pointY - centerY;
            if (offsetX >= 0)
            {
                return offsetY >= 0 ? 1 : 4;
            }
            else
            {
                return offsetY >= 0 ? 2 : 3;
            }
        }
    }
}
