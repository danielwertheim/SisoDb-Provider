namespace SisoDb.Structures.Schemas.Builders
{
    public interface ISchemaBuilder
    {
        IStructureSchema CreateSchema(IStructureType structureType);
    }
}