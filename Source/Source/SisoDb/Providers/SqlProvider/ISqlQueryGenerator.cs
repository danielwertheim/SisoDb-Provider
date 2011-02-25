using SisoDb.Querying;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public interface ISqlQueryGenerator
    {
        ISqlCommandInfo Generate(IQueryCommand queryCommand, IStructureSchema schema);

        ISqlCommandInfo GenerateWhere(IQueryCommand queryCommand, IStructureSchema schema);
    }
}