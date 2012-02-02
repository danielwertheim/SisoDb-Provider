using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using EnsureThat;
using PineCone.Structures;

namespace SisoDb.Caching
{
	[Serializable]
	public class InProcMemoryCache : ICache
	{
		private readonly Type _structureType;
		private readonly ConcurrentDictionary<IStructureId, object> _items;

		public InProcMemoryCache(Type structureType)
		{
			Ensure.That(structureType, "structureType").IsNotNull();

			_structureType = structureType;
			_items = new ConcurrentDictionary<IStructureId, object>();
		}

		public Type StructureType
		{
			get { return _structureType; }
		}

		public void Clear()
		{
			_items.Clear();
		}

		public T GetById<T>(IStructureId id) where T : class
		{
			object structure;
			_items.TryGetValue(id, out structure);
			return (T)structure;
		}

		public IDictionary<IStructureId, T> GetByIds<T>(IStructureId[] ids) where T : class
		{
			var result = new Dictionary<IStructureId, T>(ids.Length);

			foreach (var id in ids)
			{
				var structure = GetById<T>(id);
				if (structure != null)
					result.Add(id, structure);
			}

			return result;
		}

		public T Put<T>(IStructureId structureId, T structure) where T : class
		{
			_items.AddOrUpdate(structureId, structure, (k, v) => structure);

			return structure;
		}

		public IEnumerable<T> Put<T>(IEnumerable<KeyValuePair<IStructureId, T>> items) where T : class
		{
			foreach (var kv in items)
			{
				Put(kv.Key, kv.Value);
				yield return kv.Value;
			}
		}

		public void Remove(IStructureId structureId)
		{
			object temp;
			_items.TryRemove(structureId, out temp);
		}

		public void Remove(IEnumerable<IStructureId> structureIds)
		{
			foreach (var structureId in structureIds)
				Remove(structureId);
		}
	}
}