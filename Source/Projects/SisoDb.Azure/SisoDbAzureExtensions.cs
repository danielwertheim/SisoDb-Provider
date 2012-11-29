namespace SisoDb.Azure
{
    public static class SisoDbAzureExtensions
    {
        public static ISisoDatabase CreateAzureDb(this string connectionStringOrName)
        {
            var cnInfo = new SqlAzureConnectionInfo(connectionStringOrName);
            var factory = new SqlAzureDbFactory();

            return factory.CreateDatabase(cnInfo);
        }
    }
}