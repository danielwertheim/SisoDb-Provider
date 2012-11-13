using SisoDb.Dac;

namespace SisoDb.Spatial
{
	public class SpatialSqlStatements : SqlStatementsBase
    {
        public static ISqlStatements Instance { get; set; }

        static SpatialSqlStatements()
        {
            Instance = new SpatialSqlStatements();
        }

	    public SpatialSqlStatements()
            : base(typeof(SpatialSqlStatements).Assembly, "Resources.SpatialSqlStatements")
        {
        }
    }
}