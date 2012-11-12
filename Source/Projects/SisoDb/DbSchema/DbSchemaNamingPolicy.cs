using System;
using SisoDb.NCore;
using SisoDb.Resources;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public static class DbSchemaNamingPolicy
    {
        public const int MaxLenOfPrefix = 10;
        private static string _structureNamePrefix;

        public static string StructureNamePrefix
        {
            get { return _structureNamePrefix; }
            set
            {
                if(value != null && value.Length > MaxLenOfPrefix)
                    throw new ArgumentException(ExceptionMessages.DbSchemaNamingPolicy_StructureNamePrefix_IsToLong.Inject(MaxLenOfPrefix));

                _structureNamePrefix = value;
            }
        }

        public static string GenerateFor(IStructureSchema structureSchema)
        {
            return GenerateFor(structureSchema.Name);
        }

        public static string GenerateFor(string structureSchemaName)
        {
            return StructureNamePrefix == null
                       ? structureSchemaName
                       : string.Concat(StructureNamePrefix, structureSchemaName);
        }

        public static void Reset()
        {
            StructureNamePrefix = null;
        }
    }
}