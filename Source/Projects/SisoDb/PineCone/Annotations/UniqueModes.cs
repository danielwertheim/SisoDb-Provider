using System;

namespace PineCone.Annotations
{
    /// <summary>
    /// Defines how the <see cref="UniqueAttribute"/> should
    /// be applied as a constraint in the model.
    /// </summary>
    [Serializable]
    public enum UniqueModes
    {
        /// <summary>
        /// Unique per type, e.g OrderNo
        /// </summary>
        PerType = 0,
        /// <summary>
        /// Unique per instance, e.g. ProductNo in the Order.
        /// </summary>
        PerInstance = 1
    }
}