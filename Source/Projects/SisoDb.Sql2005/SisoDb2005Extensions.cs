namespace SisoDb.Sql2005
{
    public static class SisoDb2005Extensions
    {
        public static ISisoDatabase CreateSql2005Db(this string connectionStringOrName)
        {
            var cnInfo = new Sql2005ConnectionInfo(connectionStringOrName);
            var factory = new Sql2005DbFactory();

            return factory.CreateDatabase(cnInfo);
        }
    }
}