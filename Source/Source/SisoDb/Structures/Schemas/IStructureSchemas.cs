namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        IStructureSchema GetSchema<T>() where T : class;

        void RemoveSchema<T>() where T : class;
        
        void Clear();
    }
}