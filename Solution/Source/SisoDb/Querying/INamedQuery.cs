using System.Collections.Generic;

namespace SisoDb.Querying
{
    public interface INamedQuery
    {
        string Name { get; }

        IEnumerable<IQueryParameter> Parameters { get; }

        void Add(params IQueryParameter[] parameters);
    }
}