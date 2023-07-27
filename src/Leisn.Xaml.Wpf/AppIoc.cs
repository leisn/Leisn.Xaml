// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Diagnostics.CodeAnalysis;

namespace Leisn.Xaml.Wpf
{
    public static class AppIoc
    {
        public static IServiceProvider? ServiceProvider { get; private set; }

        [MemberNotNull(nameof(ServiceProvider))]
        public static void Initialize(IServiceProvider provider)
        {
            ServiceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public static object? Get(Type serviceType)
        {
            return ServiceProvider?.GetService(serviceType);
        }

        public static T? Get<T>() where T : class
        {
            return Get(typeof(T)) as T;
        }

        public static object GetRequired(Type serviceType)
        {
            return ServiceProvider?.GetService(serviceType) ?? throw new ArgumentException($"{serviceType} not found.");
        }

        public static T GetRequired<T>() where T : class
        {
            return (T)GetRequired(typeof(T));
        }
    }
}
