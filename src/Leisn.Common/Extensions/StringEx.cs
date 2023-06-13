using System.Diagnostics.CodeAnalysis;

namespace System
{
    public static class StringEx
    {
        [return: NotNullIfNotNull("source")]
        public static string Replace(this string source, int startIndex, int endIndex, string replacement)
        {
            if (source == null)
                return source!;
            return source[..startIndex] + replacement + source[(endIndex + 1)..];
        }
    }
}
