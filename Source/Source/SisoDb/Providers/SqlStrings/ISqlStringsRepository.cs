namespace SisoDb.Providers.SqlStrings
{
    public interface ISqlStringsRepository
    {
        string GetSql(string name);
    }
}