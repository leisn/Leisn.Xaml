using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Interfaces
{
    public interface IUnitConverter<in TUnit>
    {
        double Convert(double value, TUnit oldUnit, TUnit newUnit);
    }
}
