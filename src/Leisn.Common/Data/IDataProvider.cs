// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;

namespace Leisn.Common.Data
{

    public interface IDataProvider<out T>
    {
        IEnumerable<T> GetData();

        public Type GetDataType()
        {
            return typeof(T);
        }
    }
}
