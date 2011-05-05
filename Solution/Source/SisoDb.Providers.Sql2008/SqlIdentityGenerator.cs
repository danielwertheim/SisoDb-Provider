using SisoDb.Core;
using SisoDb.Providers.Dac;
using SisoDb.Structures.Schemas;

namespace SisoDb.Providers.Sql2008
{
    public class SqlIdentityGenerator : IIdentityGenerator
    {
        private readonly ISqlDbClient _dbClient;

        public SqlIdentityGenerator(ISqlDbClient dbClient)
        {
            _dbClient = dbClient.AssertNotNull("dbClient");
        }

        public int CheckOutAndGetSeed(IStructureSchema structureSchema, int numOfIds)
        {
            return _dbClient.CheckOutAndGetNextIdentity(structureSchema.Hash, numOfIds);
        }
    }
}