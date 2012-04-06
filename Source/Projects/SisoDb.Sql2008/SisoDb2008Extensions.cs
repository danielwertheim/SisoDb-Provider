namespace SisoDb.Sql2008
{
    public static class SisoDb2008Extensions
    {
        public static ISisoDatabase CreateSql2008Db(this string connectionStringOrName)
        {
            var cnInfo = new Sql2008ConnectionInfo(connectionStringOrName);
            var factory = new Sql2008DbFactory();

            return factory.CreateDatabase(cnInfo);
        }
    }
}