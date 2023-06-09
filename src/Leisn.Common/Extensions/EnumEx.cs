using System;
using System.Linq;
using System.Reflection;

namespace Leisn.Common
{
    public static class EnumEx
    {
        /// <summary>
        /// 返回 Enum 的特性
        /// </summary>
        public static T? Attr<T>(this Enum enmu) where T : Attribute
        {
            return enmu?.GetType().GetMember(enmu.ToString()).FirstOrDefault()?
                .GetCustomAttribute<T>();
        }
    }
}
