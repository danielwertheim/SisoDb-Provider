using System;

namespace SisoDb.Structures
{
	[Serializable]
	public class StructureIdInterval
	{
		public IStructureId From { get; private set; }
		public IStructureId To { get; private set; }
		public bool IsComplete { get; private set; }

		public void Clear()
		{
			From = null;
			To = null;
			IsComplete = false;
		}

		public void Set(IStructureId id)
		{
			From = From ?? id;
			To = id;
			IsComplete = From != null && To != null;
		}
	}
}