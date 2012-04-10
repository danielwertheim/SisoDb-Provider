namespace SisoDb.SqlCe4
{
    public static class SisoDbCe4Extensions
    {
        public static ISisoDatabase CreateSqlCe4Db(this string connectionStringOrName)
        {
            var cnInfo = new SqlCe4ConnectionInfo(connectionStringOrName);
            var factory = new SqlCe4DbFactory();

            return factory.CreateDatabase(cnInfo);
        }
    }
}