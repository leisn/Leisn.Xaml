// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Leisn.Xaml.Wpf
{
    public static class UIContext
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        [MemberNotNull(nameof(ServiceProvider))]
        public static void Initialize(IServiceProvider provider)
        {
            ServiceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public static object? Create(Type type) => Activator.CreateInstance(type);
        public static object? Get(Type serviceType)
        {
            var obj = ServiceProvider?.GetService(serviceType);
            if (obj is not null)
                return obj;

            var method = serviceType.GetMethod(@"GetInstance", BindingFlags.Static | BindingFlags.Public);
            if (method is not null && method.ReturnType == serviceType && method.GetParameters().Length == 0)
            {
                return method.Invoke(null, null);
            }
            var property = serviceType.GetProperty(@"Instance", BindingFlags.Static | BindingFlags.Public);
            if (property is not null && property.PropertyType == serviceType)
            {
                return property.GetValue(null);
            }
            var field = serviceType.GetField(@"Instance", BindingFlags.Static | BindingFlags.Public);
            if (field is not null && field.FieldType == serviceType)
            {
                return field.GetValue(null);
            }
            return Create(serviceType);
        }
        public static object? Get<T>() => Get(typeof(T));

    }
}
