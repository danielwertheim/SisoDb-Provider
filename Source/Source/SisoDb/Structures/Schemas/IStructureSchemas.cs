namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        IStructureSchema GetSchema<T>() where T : class;
    }
}