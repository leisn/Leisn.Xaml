﻿// @Leisn (https://leisn.com , https://github.com/leisn)

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
            return type.IsSubclassOf(parentType) || type.IsImplementOf(parentType);
        }

        /// <summary>
        /// Is type <see cref="Nullable{T}"/>, e.g. bool? , int?
        /// </summary>
        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && Equals(typeof(Nullable<>), type.GetGenericTypeDefinition());
        }

        /// <summary>
        /// Is type implment of [interfaceType], GenericTypeDefinition supported.
        /// </summary>
        /// <param name="interfaceType"> Type of interface</param>
        public static bool IsImplementOf(this Type type, Type interfaceType)
        {
            var interfaces = type.GetInterfaces();
            foreach (var item in interfaces)
            {
                if (Equals(interfaceType, item))
                {
                    return true;
                }
                if (interfaceType.IsTypeDefinition && item.IsGenericType &&
                    Equals(interfaceType, item.GetGenericTypeDefinition()))
                {
                    return true;
                }
            }
            return false;
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
