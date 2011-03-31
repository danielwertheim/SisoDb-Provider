namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        IStructureSchema GetSchema(StructureType structureType);

        void RemoveSchema(StructureType structureType);
        
        void Clear();
    }
}