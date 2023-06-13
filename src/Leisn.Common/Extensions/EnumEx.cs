using System.Linq;
using System.Reflection;

namespace System
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

        public static bool TryParse<TEnum>(object? obj, out TEnum @enum) where TEnum : struct
        {
            if (obj is TEnum e)
            {
                @enum = e;
                return true;
            }
            return Enum.TryParse(obj?.ToString(), true, out @enum);
        }
    }
}
