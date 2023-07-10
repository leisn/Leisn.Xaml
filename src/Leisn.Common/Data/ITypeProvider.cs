// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;

namespace Leisn.Common.Data
{
    public interface ITypeProvider
    {
        IEnumerable<Type> GetTypes();
    }
}
