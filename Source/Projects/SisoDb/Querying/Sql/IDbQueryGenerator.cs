using PineCone.Structures.Schemas;
using SisoDb.Commands;

namespace SisoDb.Querying.Sql
{
    public interface IDbQueryGenerator
    {
        ISqlCommandInfo Generate(IQueryCommand queryCommand, IStructureSchema schema);

        ISqlCommandInfo GenerateWhere(IQueryCommand queryCommand);
    }
}