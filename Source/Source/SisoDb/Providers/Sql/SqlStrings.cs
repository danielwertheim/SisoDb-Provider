using System;
using System.Resources;
using SisoDb.Providers.SqlProvider;

namespace SisoDb.Providers.Sql
{
    public class SqlStrings : ISqlStrings
    {
        private static readonly Type ThisType = typeof (SqlStrings);
        private static readonly ResourceManager Sql2008Strings;
        private static readonly ResourceManager AzureStrings;

        private readonly ResourceManager _primary;
        private readonly ResourceManager _secondary;

        static SqlStrings()
        {
            var prefix = ThisType.FullName + ".";
            var sql2008Resx = prefix + "2008";
            var sqlAzureResx = prefix + "Azure";    

            Sql2008Strings = new ResourceManager(sql2008Resx, ThisType.Assembly);
            AzureStrings = new ResourceManager(sqlAzureResx, ThisType.Assembly);
        }

        public SqlStrings(StorageProviders storageProvider)
        {
            //switch (storageProvider)
            //{
            //    case StorageProviders.SqlAzure:
            //        _primary = AzureStrings;
            //        _secondary = Sql2008Strings;
            //        break;
            //    default:
            _primary = Sql2008Strings;
            _secondary = _primary;
            //        break;
            //}
        }

        public string GetSql(string name)
        {
            return _primary.GetString(name) ?? _secondary.GetString(name);
        }
    }
}