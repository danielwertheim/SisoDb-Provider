using System;
using System.Configuration;
using SisoDb.EnsureThat;

namespace SisoDb.Configurations
{
    [Serializable]
    public static class ConnectionString
    {
        public static string Get(string connectionStringOrName)
        {
            Ensure.That(connectionStringOrName, "connectionStringOrName").IsNotNullOrWhiteSpace();

            var config =
                ConfigurationManager.ConnectionStrings[string.Concat(Environment.MachineName, "_", connectionStringOrName)]
                ?? ConfigurationManager.ConnectionStrings[connectionStringOrName];

            return config == null
                ? connectionStringOrName
                : config.ConnectionString;
        }
    }
}