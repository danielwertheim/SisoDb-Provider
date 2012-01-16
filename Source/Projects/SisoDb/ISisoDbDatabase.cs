using SisoDb.DbSchema;

namespace SisoDb
{
	/// <summary>
	/// Represents a database targetting a plain database and not e.g
	/// an in-memory database.
	/// </summary>
	public interface ISisoDbDatabase : ISisoDatabase
	{
		IDbProviderFactory ProviderFactory { get; }

		IDbSchemaManager SchemaManager { get; }
	}
}