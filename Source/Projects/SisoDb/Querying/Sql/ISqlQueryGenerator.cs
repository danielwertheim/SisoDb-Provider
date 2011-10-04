using PineCone.Structures.Schemas;
using SisoDb.Commands;

namespace SisoDb.Querying.Sql
{
    public interface ISqlQueryGenerator
    {
        ISqlCommandInfo Generate(IQueryCommand queryCommand, IStructureSchema schema);

        ISqlCommandInfo GenerateWhere(IQueryCommand queryCommand);
    }
}