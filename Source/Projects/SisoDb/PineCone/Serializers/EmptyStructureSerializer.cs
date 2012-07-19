namespace SisoDb.PineCone.Serializers
{
    public class EmptyStructureSerializer : IStructureSerializer
    {
        public string Serialize<T>(T item) where T : class
        {
            return null;
        }
    }
}