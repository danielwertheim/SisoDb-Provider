namespace SisoDb
{
    public static class StructureTypeNameFor<T> where T : class
    {
        public static readonly string Name = TypeFor<T>.Type.Name;
    }
}