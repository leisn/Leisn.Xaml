// @Leisn (https://leisn.com , https://github.com/leisn)

using System;

namespace Leisn.Common
{
    /// <inheritdoc/>
    public interface ICloneable<T> : ICloneable
    {
        /// <summary>
        /// Creates a new {T} that is a copy of the current instance.
        /// </summary>
        /// <returns> A new {T} that is a copy of this instance.</returns>
        new T Clone();
        object ICloneable.Clone() => Clone()!;
    }
}
