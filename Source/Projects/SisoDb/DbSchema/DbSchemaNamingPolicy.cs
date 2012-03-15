using System;
using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public static class DbSchemaNamingPolicy
    {
        public static Func<IStructureSchema, string> StructureNameGenerator;

        static DbSchemaNamingPolicy()
        {
            StructureNameGenerator = schema => schema.Name;
        }
    }
}