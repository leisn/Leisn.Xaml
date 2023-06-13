// By Leisn (https://leisn.com , https://github.com/leisn)

using System.Diagnostics.CodeAnalysis;

namespace Leisn.Common.Extensions
{
    public static class StringEx
    {
        [return: NotNullIfNotNull("source")]
        public static string? Replace(this string source, int startIndex, int endIndex, string replacement)
        {
            return source == null ? source : source[..startIndex] + replacement + source[(endIndex + 1)..];
        }
    }
}
