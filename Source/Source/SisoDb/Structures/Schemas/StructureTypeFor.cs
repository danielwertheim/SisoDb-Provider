namespace SisoDb.Structures.Schemas
{
    public static class StructureTypeFor<T> where T : class
    {
        public static readonly IStructureType Instance;

        static StructureTypeFor()
        {
            Instance = new StructureType(typeof(T));
        }
    }
}