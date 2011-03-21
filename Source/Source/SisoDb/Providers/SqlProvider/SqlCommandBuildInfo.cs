using System;
using System.Collections.Generic;
using SisoDb.Querying;

namespace SisoDb.Providers.SqlProvider
{
    [Serializable]
    internal class SqlCommandBuildInfo
    {
        public string StructureTableName { get; set; }
        public string IndexesTableName { get; set; }
        public string TakeSql { get; set; }
        public string OrderBySql { get; set; }
        public string IncludesSql { get; set; }
        public string WhereSql { get; set; }
        public IList<IQueryParameter> WhereParams { get; set; }
    }
}