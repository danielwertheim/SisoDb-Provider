using System;

namespace SisoDb
{
	[Serializable]
	public enum StructureSetMigratorStatuses
	{
		/// <summary>
		/// Indicates that you want to keep the structure
		/// and the new-structure will be persisted with
		/// the same id as the old.
		/// </summary>
		Keep,
		/// <summary>
		/// Indicates that your are no longer interested
		/// in keeping the structure, hence it will be
		/// deleted.
		/// </summary>
		Trash,
		/// <summary>
		/// Abort and rollback the process.
		/// </summary>
		Abort
	}
}