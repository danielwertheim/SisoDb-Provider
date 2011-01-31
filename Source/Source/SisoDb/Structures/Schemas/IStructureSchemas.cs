namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchemas
    {
        void Register<T>() where T : class;

        IStructureSchema GetSchema<T>() where T : class;
    }
}