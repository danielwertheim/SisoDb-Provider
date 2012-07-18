namespace SisoDb.PineCone.Structures.Schemas.MemberAccessors
{
    public interface IIdAccessor : IMemberAccessor
    {
        StructureIdTypes IdType { get; }

        IStructureId GetValue<T>(T item) where T : class;

        void SetValue<T>(T item, IStructureId value) where T : class;
    }
}