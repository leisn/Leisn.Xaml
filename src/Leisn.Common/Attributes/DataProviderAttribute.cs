using Leisn.Common.Interfaces;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Leisn.Common.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DataProviderAttribute : Attribute
    {
        public Type ProviderType { get; }

        public DataProviderAttribute(Type providerType)
        {
            ProviderType = providerType;
            if (!typeof(IDataProvider).IsAssignableFrom(providerType))
            {
                throw new NotSupportedException($"{providerType} is not IDataProvider");
            }
        }
    }
}
