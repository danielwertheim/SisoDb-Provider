namespace SisoDb.Lambdas.Nodes
{
    internal class NameStrategy
    {
        public string Apply(string value)
        {
            return value != "Id" ? value : "StructureId";
        }
    }
}