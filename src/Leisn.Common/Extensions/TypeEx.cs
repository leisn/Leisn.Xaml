// @Leisn (https://leisn.com , https://github.com/leisn)

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System
{
    public static class TypeEx
    {
        /// <summary>
        /// Is derived or implement of [parentType]
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parentType">Parent class or interface type</param>
        /// <returns></returns>
        public static bool IsTypeOf(this Type type, Type parentType)
        {
            return Equals(type, parentType) || type.IsDerivedFrom(parentType) || type.IsImplementOf(parentType);
        }

        /// <summary>
        /// Is derived from [parentType], if is same type return true
        /// </summary>
        public static bool IsDerivedFrom(this Type type, Type parentType)
        {
            if (Equals(type, parentType) || type.IsSubclassOf(parentType))
                return true;

            var parentTypeDefinition = parentType.IsGenericType ? parentType.GetGenericTypeDefinition() : null;
            if (type.IsGenericType && Equals(parentTypeDefinition, type.GetGenericTypeDefinition()))
                return true;

            var inheritType = type.GetInheritTypes();
            foreach (var item in inheritType)
            {
                if (Equals(item, parentType))
                    return true;
                if (item.IsGenericType && Equals(parentTypeDefinition, item.GetGenericTypeDefinition()))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Is type implment of [interfaceType], GenericTypeDefinition supported.
        /// </summary>
        /// <param name="interfaceType"> Type of interface</param>
        public static bool IsImplementOf(this Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            var interfaceTypeDefinition = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : null;
            foreach (var item in interfaces)
            {
                if (Equals(interfaceType, item))
                {
                    return true;
                }
                if (item.IsGenericType && Equals(interfaceTypeDefinition, item.GetGenericTypeDefinition()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get parent types of current type 
        /// </summary>
        public static List<Type> GetInheritTypes(this Type type)
        {
            var list = new List<Type>();
            Type temp = type;
            while (temp.BaseType != null)
            {
                list.Add(temp.BaseType);
                temp = temp.BaseType;
            }
            return list;
        }

        /// <summary>
        /// Is type <see cref="Nullable{T}"/>, e.g. bool? , int?
        /// </summary>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && Equals(typeof(Nullable<>), type.GetGenericTypeDefinition());
        }

        /// <summary>
        /// Is type <see cref="Nullable{T}"/>, e.g. bool? , int?
        /// </summary>
        public static bool IsNullable(this Type type, out Type? elementType)
        {
            elementType = Nullable.GetUnderlyingType(type);
            return elementType != null;
        }

        /// <summary>
        /// Return generic type of interface of given type definition.<br/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static Type? GetGenericInterfaceTypeOf(this Type type, Type interfaceGenericTypeDefinition)
        {
            if (!interfaceGenericTypeDefinition.IsTypeDefinition)
                throw new ArgumentException($"{interfaceGenericTypeDefinition} not type definition", nameof(interfaceGenericTypeDefinition));
            var interfaces = type.GetInterfaces();
            foreach (var item in interfaces)
            {
                if (item.IsGenericType &&
                    Equals(interfaceGenericTypeDefinition, item.GetGenericTypeDefinition()))
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        ///  Is type implment of <see cref="IEnumerable"/>, Notice <see cref="string"/> also return true.
        /// </summary>
        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Is type implment of <see cref="IEnumerable{T}"/> />, Notice <see cref="string"/> also return true.
        /// </summary>
        public static bool IsEnumerableT(this Type type)
        {
            return type.IsImplementOf(typeof(IEnumerable<>));
        }

        /// <summary>
        /// Is type for Numeric, Notice Enum return false
        /// </summary>
        public static bool IsNumericType(this Type type)
        {
            if (!type.IsValueType || type.IsEnum)
                return false;
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte
                or TypeCode.Decimal
                or TypeCode.Double
                or TypeCode.Int16
                or TypeCode.Int32
                or TypeCode.Int64
                or TypeCode.SByte
                or TypeCode.Single
                or TypeCode.UInt16
                or TypeCode.UInt32
                or TypeCode.UInt64 => true,
                _ => false,
            };
        }

        public static bool IsValueTuple(this Type type)
        {
            if (!type.IsConstructedGenericType || !type.IsValueType)
                return false;
            var typeDef = type.GetGenericTypeDefinition();
            return Equals(typeDef, typeof(ValueTuple<>))
                     || Equals(typeDef, typeof(ValueTuple<,>))
                     || Equals(typeDef, typeof(ValueTuple<,,>))
                     || Equals(typeDef, typeof(ValueTuple<,,,>))
                     || Equals(typeDef, typeof(ValueTuple<,,,,>))
                     || Equals(typeDef, typeof(ValueTuple<,,,,,>))
                     || Equals(typeDef, typeof(ValueTuple<,,,,,,>))
                     || Equals(typeDef, typeof(ValueTuple<,,,,,,,>));
        }

        public static bool IsTuple(this Type type)
        {
            if (!type.IsConstructedGenericType || type.IsValueType)
                return false;
            var typeDef = type.GetGenericTypeDefinition();
            return Equals(typeDef, typeof(Tuple<>))
                     || Equals(typeDef, typeof(Tuple<,>))
                     || Equals(typeDef, typeof(Tuple<,,>))
                     || Equals(typeDef, typeof(Tuple<,,,>))
                     || Equals(typeDef, typeof(Tuple<,,,,>))
                     || Equals(typeDef, typeof(Tuple<,,,,,>))
                     || Equals(typeDef, typeof(Tuple<,,,,,,>))
                     || Equals(typeDef, typeof(Tuple<,,,,,,,>));
        }

        public static string GetShortName(this Type type)
        {
            if (type.IsGenericType)
            {
                return $"{type.Name[..^2]}<{string.Join(", ", type.GetGenericArguments().Select(x => x.GetShortName()))}>";
            }
            return type.Name;
        }
    }
}
