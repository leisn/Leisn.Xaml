// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Leisn.NodeEditor
{
    public static class GeometryHepler
    {
        /// <summary>
        /// 转换为弧度
        /// </summary>
        public static float ToRadians(float angle) => angle / 180 * MathF.PI;
        /// <summary>
        /// 转换为角度
        /// </summary>
        public static float ToAngle(float radians) => radians * 180 / MathF.PI;

        /// <summary>
        /// 把局部变换到父级变换的坐标系下（子变换->父变换）
        /// </summary>
        /// <param name="self">局部变换</param>
        /// <param name="localTransform">父级变换</param>
        /// <returns>子变换->父变换</returns>
        public static Matrix3x2 ToWorld(this Matrix3x2 self, Matrix3x2 wordTransform) => wordTransform * self;
    }
}
