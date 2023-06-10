﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Interfaces
{
    public interface IDataDeclaration<out T>
    {
        T Value { get; }
        string DisplayName { get; }
        string Description { get; }
    }
}