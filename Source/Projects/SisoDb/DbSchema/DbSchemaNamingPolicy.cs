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

		public static string GetStructureTableName(this IStructureSchema structureSchema)
		{
			return string.Concat(StructureNameGenerator.Invoke(structureSchema), "Structure");
		}

		public static IndexesTableNames GetIndexesTableNames(this IStructureSchema structureSchema)
		{
			return new IndexesTableNames(structureSchema);
		}

		public static string GetIndexesTableNameFor(this IStructureSchema structureSchema, IndexesTypes type)
		{
			return string.Concat(StructureNameGenerator.Invoke(structureSchema), type.ToString());
		}

		public static string GetUniquesTableName(this IStructureSchema structureSchema)
		{
			return string.Concat(StructureNameGenerator.Invoke(structureSchema), "Uniques");
		}
	}
}