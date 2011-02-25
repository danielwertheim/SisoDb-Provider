namespace SisoDb.Lambdas.Processors
{
    public class NameStrategy : INameStrategy
    {
        public string Apply(string value)
        {
            return value != "Id" ? value : "StructureId";
        }
    }
}