namespace SisoDb.Lambdas.Processors
{
    public interface INameStrategy
    {
        string Apply(string value);
    }
}