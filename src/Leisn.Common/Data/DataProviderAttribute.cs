// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Data
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DataProviderAttribute : Attribute
    {
        public Type ProviderType { get; }

        public DataProviderAttribute(Type providerType)
        {
            ProviderType = providerType;
            if (!typeof(IDataProvider<object>).IsAssignableFrom(providerType))
            {
                throw new NotSupportedException($"{providerType} is not IDataProvider");
            }
            //if (providerType.GetConstructor(Type.EmptyTypes) == null)
            //{
            //    throw new NotSupportedException($"{providerType} must have non-paramter constructor");
            //}
        }
    }
}
