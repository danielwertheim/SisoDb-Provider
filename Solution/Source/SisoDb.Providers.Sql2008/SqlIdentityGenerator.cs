using SisoDb.Core;
using SisoDb.Providers.Sql2008.Dac;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008
{
    public class SqlIdentityGenerator : IIdentityGenerator
    {
        private readonly SqlDbClient _dbClient;

        public SqlIdentityGenerator(SqlDbClient dbClient)
        {
            _dbClient = dbClient.AssertNotNull("dbClient");
        }

        public int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds)
        {
            return _dbClient.CheckOutAndGetNextIdentity(structureSchema.Hash, numOfIds);
        }
    }
}