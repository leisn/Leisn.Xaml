// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Data
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TypeProviderAttribute : Attribute
    {
        public Type ProviderType { get; }

        public TypeProviderAttribute(Type providerType)
        {
            ProviderType = providerType;
            if (!typeof(ITypeProvider).IsAssignableFrom(providerType))
            {
                throw new NotSupportedException($"{providerType} is not ITypeProvider");
            }
        }
    }
}
