using System.Collections.Generic;
using SisoDb.Dac;

namespace SisoDb.Commands
{
    public interface ISqlCommandInfo
    {
        string Sql { get; }
        IList<IDacParameter> Parameters { get; }
    }
}