using System.Collections.Generic;
using SisoDb.Dac;

namespace SisoDb.Querying
{
    public interface INamedQuery
    {
        string Name { get; }

        IEnumerable<IDacParameter> Parameters { get; }

        void Add(params IDacParameter[] parameters);
    }
}