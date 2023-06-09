using System;
using System.Collections.Generic;

namespace Leisn.Common.Interfaces
{

    public interface IDataProvider<out T>
    {
        IEnumerable<T> GetData();

        public Type GetDataType() => typeof(T);
    }
}
