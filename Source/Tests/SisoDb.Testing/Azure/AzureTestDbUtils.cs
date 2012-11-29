using SisoDb.Dac;

namespace SisoDb.Testing.Azure
{
    public class AzureTestDbUtils : SqlServerTestDbUtils
    {
        public AzureTestDbUtils(IAdoDriver driver, string connectionString)
            : base(driver, connectionString)
        {
        }
    }
}