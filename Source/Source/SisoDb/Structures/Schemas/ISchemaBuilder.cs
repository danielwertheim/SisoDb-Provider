namespace SisoDb.Structures.Schemas
{
    internal interface ISchemaBuilder<T> where T : class 
    {
        IStructureSchema CreateSchema();
    }
}