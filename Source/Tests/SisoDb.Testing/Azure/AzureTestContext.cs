using SisoDb.Azure;

namespace SisoDb.Testing.Azure
{
    public class AzureTestContext : TestContextBase
    {
        public AzureTestContext(string connectionStringName)
            : base(connectionStringName.CreateAzureDb())
        {
            DbHelper = new AzureTestDbUtils(Database.ProviderFactory.GetAdoDriver(), Database.ConnectionInfo.ClientConnectionString);
        }
    }
}