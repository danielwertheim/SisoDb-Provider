using System;
using System.Linq;
using NCore.Collections;
using PineCone.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public static class DbSchemas
    {
        public static class Suffixes
        {
            public static readonly string[] AllSuffixes;
            public const string StructureTableNameSuffix = "Structure";
            public const string UniquesTableNameSuffix = "Uniques";
            public static readonly string[] IndexesTableNameSuffixes;

            static Suffixes()
            {
                IndexesTableNameSuffixes = Enum.GetNames(typeof(IndexesTypes));
                AllSuffixes = new[] { StructureTableNameSuffix, UniquesTableNameSuffix }.MergeWith(IndexesTableNameSuffixes).ToArray();
            }
        }

        public static class SysTables
        {
            public const string IdentitiesTable = "SisoDbIdentities";
        }

        public static string GetStructureTableName(this IStructureSchema structureSchema)
        {
            return string.Concat(DbSchemaNamingPolicy.StructureNameGenerator.Invoke(structureSchema), Suffixes.StructureTableNameSuffix);
        }

        public static IndexesTableNames GetIndexesTableNames(this IStructureSchema structureSchema)
        {
            return new IndexesTableNames(structureSchema);
        }

        public static string GetIndexesTableNameFor(this IStructureSchema structureSchema, IndexesTypes type)
        {
            return string.Concat(DbSchemaNamingPolicy.StructureNameGenerator.Invoke(structureSchema), type.ToString());
        }

        public static string GetUniquesTableName(this IStructureSchema structureSchema)
        {
            return string.Concat(DbSchemaNamingPolicy.StructureNameGenerator.Invoke(structureSchema), Suffixes.UniquesTableNameSuffix);
        }
    }
}