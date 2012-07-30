using System;

namespace SisoDb.PineCone.Structures.Schemas
{
    public interface IStructureType
    {
    	Type Type { get; } 
        string Name { get; }
        IStructureProperty IdProperty { get; }
        IStructureProperty TimeStampProperty { get; }
        IStructureProperty ConcurrencyTokenProperty { get; }
		IStructureProperty[] IndexableProperties { get; }
        IStructureProperty[] ContainedStructureProperties { get; }
    }
}