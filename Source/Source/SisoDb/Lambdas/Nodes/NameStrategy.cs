namespace SisoDb.Lambdas.Nodes
{
    public class NameStrategy
    {
        public string Apply(string value)
        {
            return value != "Id" ? value : "StructureId";
        }
    }
}