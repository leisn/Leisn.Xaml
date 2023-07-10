// @Leisn (https://leisn.com , https://github.com/leisn)

namespace System
{
    public static class TypeEx
    {
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && Equals(typeof(Nullable<>), type.GetGenericTypeDefinition());
        }

        public static bool IsClassOrNullable(this Type type)
        {
            return type.IsClass || type.IsNullable();
        }
    }
}
