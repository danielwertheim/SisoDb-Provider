namespace SisoDb
{
    //TODO: Move to PineCone
    public static class StructureTypeNameFor<T> where T : class
    {
        public static readonly string Name = TypeFor<T>.Type.Name;
    }
}