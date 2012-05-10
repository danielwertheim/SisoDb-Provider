namespace SisoDb.Configurations
{
    public static class ConfigurationExtensions
    {
         public static DbConfiguration Configure(this ISisoDatabase database)
         {
             return new DbConfiguration(database);
         }
    }
}