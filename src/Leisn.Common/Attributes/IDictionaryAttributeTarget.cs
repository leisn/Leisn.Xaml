// @Leisn (https://leisn.com , https://github.com/leisn)

using System;
using System.Collections.Generic;
using System.Text;

namespace Leisn.Common.Attributes
{
    public enum DictionaryTarget
    {
        Value,
        Key,
        Both
    }

    public interface IDictionaryAttributeTarget
    {
        DictionaryTarget DictionaryTarget { get; }
    }

}
