namespace SisoDb.Structures.Schemas
{
    public static class AutoSchemaBuilderFor<T>
        where T : class
    {
        public static readonly ISchemaBuilder Instance;

        static AutoSchemaBuilderFor()
        {
            Instance = new AutoSchemaBuilder(StructureTypeFor<T>.Instance);
        }
    }
}