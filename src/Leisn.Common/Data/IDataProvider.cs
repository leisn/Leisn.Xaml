using System;
using System.Collections.Generic;

namespace Leisn.Common.Data
{

    public interface IDataProvider<out T>
    {
        IEnumerable<T> GetData();

        public Type GetDataType() => typeof(T);
    }
}
