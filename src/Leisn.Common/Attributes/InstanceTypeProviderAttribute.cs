// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

using Leisn.Common.Data;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class InstanceTypeProviderAttribute : Attribute
    {
        public Type ProviderType { get; }

        public InstanceTypeProviderAttribute(Type providerType)
        {
            ProviderType = providerType;
            if (!providerType.IsImplementOf(typeof(IDataProvider<Type>)))
            {
                throw new NotSupportedException($"{providerType} is not IDataProvider<Type>");
            }
        }
    }
}
