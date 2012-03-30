using System;

namespace SisoDb.DbSchema
{
    public static class DbSchemaNamingPolicy
    {
        private static readonly Func<string, string> DefaultStructureNameGenerator;
 
        public static Func<string, string> StructureNameGenerator;

        static DbSchemaNamingPolicy()
        {
            DefaultStructureNameGenerator = name => name;
            StructureNameGenerator = DefaultStructureNameGenerator;
        }

        public static void Reset()
        {
            StructureNameGenerator = DefaultStructureNameGenerator;
        }
    }
}