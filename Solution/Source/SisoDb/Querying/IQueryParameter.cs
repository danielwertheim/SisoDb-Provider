using System;

namespace SisoDb.Querying
{
    public interface IQueryParameter : IEquatable<IQueryParameter>
    {
        string Name { get; }
        object Value { get; }
    }
}