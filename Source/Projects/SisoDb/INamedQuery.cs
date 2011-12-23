using System.Collections.Generic;
using SisoDb.Dac;

namespace SisoDb
{
    public interface INamedQuery
    {
        string Name { get; }

        IEnumerable<IDacParameter> Parameters { get; }

        void Add(params IDacParameter[] parameters);
    }
}