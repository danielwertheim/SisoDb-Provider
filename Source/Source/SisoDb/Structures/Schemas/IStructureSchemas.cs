namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        IStructureSchema GetSchema(IStructureType structureType);

        void RemoveSchema(IStructureType structureType);
        
        void Clear();
    }
}