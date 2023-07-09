// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Data
{
    public interface ITypeProvider
    {
        IEnumerable<Type> GetTypes();
    }
}
