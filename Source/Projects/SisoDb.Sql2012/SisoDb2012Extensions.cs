namespace SisoDb.Sql2012
{
    public static class SisoDb2012Extensions
    {
        public static ISisoDatabase CreateSql2012Db(this string connectionStringOrName)
        {
            var cnInfo = new Sql2012ConnectionInfo(connectionStringOrName);
            var factory = new Sql2012DbFactory();

            return factory.CreateDatabase(cnInfo);
        }
    }
}