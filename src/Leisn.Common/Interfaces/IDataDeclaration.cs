using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Interfaces
{
    public interface IDataDeclaration
    {
        object Value { get; }

        string DisplayName { get; }
        string Description { get; }
    }

    public interface IDataDeclaration<T> : IDataDeclaration
    {
        new T Value { get; }
    }
}
