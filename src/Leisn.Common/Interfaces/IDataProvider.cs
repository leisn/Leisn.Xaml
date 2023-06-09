using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Interfaces
{

    public interface IDataProvider
    {
        IEnumerable<object> GetData();
    }

    public interface IDataProvider<T> : IDataProvider
    {
        new IEnumerable<T> GetData();
    }
}
