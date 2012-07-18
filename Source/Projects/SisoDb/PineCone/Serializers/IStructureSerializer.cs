namespace SisoDb.PineCone.Serializers
{
    public interface IStructureSerializer
    {
        string Serialize<T>(T item) where T : class;
    }
}