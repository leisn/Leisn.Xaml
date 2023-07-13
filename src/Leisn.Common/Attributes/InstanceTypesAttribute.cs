// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class InstanceTypesAttribute : Attribute
    {
        public Type[] InstanceTypes { get; }
        public InstanceTypesAttribute(params Type[] instanceTypes)
        {
            InstanceTypes = instanceTypes;
        }
    }
}
