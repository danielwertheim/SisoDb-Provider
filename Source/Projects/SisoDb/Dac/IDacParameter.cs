using System;

namespace SisoDb.Dac
{
    public interface IDacParameter : IEquatable<IDacParameter>
    {
        string Name { get; }
        object Value { get; }
    }
}