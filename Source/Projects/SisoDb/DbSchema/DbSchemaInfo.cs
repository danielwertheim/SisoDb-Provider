using System;
using System.Linq;
using SisoDb.Dac;
using SisoDb.NCore.Collections;
using SisoDb.Structures.Schemas;

namespace SisoDb.DbSchema
{
    public static class DbSchemaInfo
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

        public static class Parameters
        {
            public const string DbNameParamPrefix = "@dbName";
            public const string TableNameParamPrefix = "@tableName";
            public const string EntityNameParamPrefix = "@entityName";
            public static readonly string JsonParam;
            public static readonly string StringValueForValueTypeIndexParamName;
            
            static Parameters()
            {
                JsonParam = string.Concat("@", StructureStorageSchema.Fields.Json.Name);
                StringValueForValueTypeIndexParamName = string.Concat("@", IndexStorageSchema.Fields.StringValue.Name);
            }

            public static bool ShouldBeMultivalue(IDacParameter param)
            {
                return param is ArrayDacParameter;
            }

            public static bool ShouldBeDateTime(IDacParameter param)
            {
                return param.Value is DateTime;
            }

            public static bool ShouldBeNonUnicodeString(IDacParameter param)
            {
                return param.Name.StartsWith(DbNameParamPrefix, Sys.StringComparision)
                    || param.Name.StartsWith(TableNameParamPrefix, Sys.StringComparision)
                    || param.Name.StartsWith(EntityNameParamPrefix, Sys.StringComparision)
                    || param.Name.Equals(StringValueForValueTypeIndexParamName);
            }

            public static bool ShouldBeUnicodeString(IDacParameter param)
            {
                return param.Value is string;
            }

            public static bool ShouldBeJson(IDacParameter param)
            {
                return param.Name.Equals(JsonParam, Sys.StringComparision);
            }
        }

        public static string GetStructureTableName(this IStructureSchema structureSchema)
        {
            return GenerateStructureTableName(structureSchema.Name);
        }

        public static string GenerateStructureTableName(string structureName)
        {
            return string.Concat(DbSchemaNamingPolicy.GenerateFor(structureName), Suffixes.StructureTableNameSuffix);
        }

        public static IndexesTableNames GetIndexesTableNames(this IStructureSchema structureSchema)
        {
            return new IndexesTableNames(structureSchema);
        }

        public static string GenerateIndexesTableNameFor(string structureName, IndexesTypes type)
        {
            return string.Concat(DbSchemaNamingPolicy.GenerateFor(structureName), type.ToString());
        }

        public static string GetIndexesTableNameFor(this IStructureSchema structureSchema, IndexesTypes type)
        {
            return GenerateIndexesTableNameFor(structureSchema.Name, type);
        }

        public static string GetUniquesTableName(this IStructureSchema structureSchema)
        {
            return GenerateUniquesTableName(structureSchema.Name);
        }

        public static string GenerateUniquesTableName(string structureName)
        {
            return string.Concat(DbSchemaNamingPolicy.GenerateFor(structureName), Suffixes.UniquesTableNameSuffix);
        }
    }
}