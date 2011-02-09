using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    internal interface ISqlQueryGenerator
    {
        ISqlQuery Generate<T>(IQueryCommand<T> queryCommand, IStructureSchema schema) where T : class;
    }
}