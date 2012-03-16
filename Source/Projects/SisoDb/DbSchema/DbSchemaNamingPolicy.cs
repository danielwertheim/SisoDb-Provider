using System;

namespace SisoDb.DbSchema
{
    public static class DbSchemaNamingPolicy
    {
        public static Func<string, string> StructureNameGenerator;

        static DbSchemaNamingPolicy()
        {
            StructureNameGenerator = name => name;
        }
    }
}