using SisoDb.Dac;

namespace SisoDb.Spatial
{
	public class SpatialSqlStatements : SqlStatementsBase
    {
        public SpatialSqlStatements()
            : base(typeof(SpatialSqlStatements).Assembly, "Resources.SpatialSqlStatements")
        {
        }
    }
}