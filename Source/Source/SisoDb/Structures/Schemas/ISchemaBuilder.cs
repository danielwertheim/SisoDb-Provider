namespace SisoDb.Structures.Schemas
{
    public interface ISchemaBuilder<T> where T : class 
    {
        IStructureSchema CreateSchema();
    }
}