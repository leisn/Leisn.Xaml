// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

using Leisn.Common.Data;

namespace Leisn.Common.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DataProviderAttribute : Attribute, IDictionaryAttributeTarget
    {
        public Type ProviderType { get; }

        public DictionaryTarget DictionaryTarget { get; set; }

        public DataProviderAttribute(Type providerType)
        {
            ProviderType = providerType;
            if (!typeof(IDataProvider<object>).IsAssignableFrom(providerType))
            {
                throw new NotSupportedException($"{providerType} is not IDataProvider");
            }
        }
    }

}
