using SisoDb.Providers.Sql2008Provider;
using SisoDb.Structures.Schemas;

namespace SisoDb.Querying.Sql
{
    public interface ISqlQueryGenerator
    {
        ISqlCommandInfo Generate(IQueryCommand queryCommand, IStructureSchema schema);

        ISqlCommandInfo GenerateWhere(IQueryCommand queryCommand);
    }
}