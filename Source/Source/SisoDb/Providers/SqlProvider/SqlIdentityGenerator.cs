using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.SqlProvider
{
    public class SqlIdentityGenerator : IIdentityGenerator
    {
        private readonly ISisoConnectionInfo _connectionInfo;

        public SqlIdentityGenerator(ISisoConnectionInfo connectionInfo)
        {
            _connectionInfo = connectionInfo;
        }

        public int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds)
        {
            using(var dbClient = new SqlDbClient(_connectionInfo, false))
            {
                return dbClient.GetIdentity(structureSchema.Hash, numOfIds);    
            }
        }
    }
}