﻿// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NumericUpDownAttribute : Attribute
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public double Increment { get; set; }

        public NumericUpDownAttribute() { }

        public NumericUpDownAttribute(double minimum, double maximum, double increment)
        {
            Minimum = minimum;
            Maximum = maximum;
            Increment = increment;
        }
    }
}
