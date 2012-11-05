namespace SisoDb
{
    public static class DbSettingsExtensions
    {
        public static bool AllowsAnyDynamicSchemaChanges(this IDbSettings settings)
        {
            return settings.AllowDynamicSchemaCreation || settings.AllowDynamicSchemaUpdates;
        }
    }
}