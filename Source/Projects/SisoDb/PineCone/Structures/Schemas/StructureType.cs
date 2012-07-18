using System;
using SisoDb.EnsureThat;

namespace SisoDb.PineCone.Structures.Schemas
{
    [Serializable]
    public class StructureType : IStructureType
    {
		public Type Type { get; private set; }

    	public string Name
    	{
			get { return Type.Name; }
    	}

        public IStructureProperty IdProperty { get; private set; }

        public IStructureProperty TimeStampProperty { get; private set; }

        public IStructureProperty ConcurrencyTokenProperty { get; private set; }

        public IStructureProperty[] IndexableProperties { get; private set; }

        public StructureType(Type type, IStructureProperty idProperty = null, IStructureProperty concurrencyTokenProperty = null, IStructureProperty timeStampProperty = null, IStructureProperty[] indexableProperties = null)
        {
			Ensure.That(type, "type").IsNotNull();

			Type = type;
            IdProperty = idProperty;
            ConcurrencyTokenProperty = concurrencyTokenProperty;
            TimeStampProperty = timeStampProperty;
            IndexableProperties = indexableProperties ?? new IStructureProperty[]{};
        }
    }
}