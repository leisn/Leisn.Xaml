// @Leisn (https://leisn.com , https://github.com/leisn)

namespace Leisn.Common.Data
{
    public interface IDataDeclaration<out T>
    {
        T Value { get; }
        string DisplayName { get; }
        string Description { get; }
    }
}
