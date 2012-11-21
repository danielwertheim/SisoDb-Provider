using System;

namespace SisoDb
{
	/// <summary>
	/// Defines a Cache provider that can be hooked in to
	/// a <see cref="ISisoDatabase"/>.
	/// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// If <see cref="Handles"/> returns true for <paramref name="type"/>
		/// then this property should always return an instance of <see cref="ICache"/>.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ICache this[Type type] { get; }

		/// <summary>
		/// Gets or Sets a value, determining if a cache should be
		/// created automatically for a type, when the cache is
		/// consumed for the first time.
		/// </summary>
		bool AutoEnable { get; set; }

		/// <summary>
		/// Clears all <see cref="ICache"/> instances associated with
		/// this provider.
		/// </summary>
		void Clear();

		/// <summary>
		/// Indicates if the <paramref name="type"/> is
		/// supported or not by the cache provider.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		bool Handles(Type type);

		/// <summary>
		/// Enables caching for a certain structure type.
		/// </summary>
		/// <param name="type"></param>
		void EnableFor(Type type);

		/// <summary>
		/// Disables and removes caching for a certain structure type.
		/// </summary>
		/// <param name="type"></param>
		void DisableFor(Type type);
	}
}