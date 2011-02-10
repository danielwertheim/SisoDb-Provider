using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    internal interface ISqlQueryGenerator
    {
        ISqlCommandInfo Generate<T>(IQueryCommand<T> queryCommand, IStructureSchema schema) where T : class;

        ISqlCommandInfo GenerateWhere<T>(IQueryCommand<T> queryCommand, IStructureSchema schema) where T : class;
    }
}