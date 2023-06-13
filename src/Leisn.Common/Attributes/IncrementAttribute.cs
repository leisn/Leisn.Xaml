// By Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class IncrementAttribute : Attribute
    {
        public double Increment { get; }

        public IncrementAttribute(double increment)
        {
            Increment = increment;
        }
    }
}
