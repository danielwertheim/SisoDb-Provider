using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public interface IDbPipe
    {
        string Writing(IStructureSchema structureSchema, string data);
        string Reading(IStructureSchema structureSchema, string data);
    }
}