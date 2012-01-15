using System;

namespace SisoDb
{
	/// <summary>
	/// Used to update and assist you with migrating a complete structure set.
	/// </summary>
	public interface IStructureSetMigrator
	{
		void Migrate<TOld, TNew>(Func<TOld, TNew, StructureSetMigratorStatuses> modifier)
			where TOld : class
			where TNew : class;
	}
}