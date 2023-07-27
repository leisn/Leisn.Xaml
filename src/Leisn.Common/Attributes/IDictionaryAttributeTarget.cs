// @Leisn (https://leisn.com , https://github.com/leisn)

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
