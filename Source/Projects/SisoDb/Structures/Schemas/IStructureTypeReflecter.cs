using System;
using System.Collections.Generic;

namespace SisoDb.Structures.Schemas
{
    /// <summary>
    /// Responsible for identifying the Properties that should be used as
    /// StructureId, TimeStamp, ConcurrencyToken and plain indexes for a
    /// certain structure type.
    /// </summary>
    public interface IStructureTypeReflecter
    {
        IStructurePropertyFactory PropertyFactory { set; }

        bool HasIdProperty(Type structureType);
        bool HasConcurrencyTokenProperty(Type structureType);
        bool HasTimeStampProperty(Type structureType);

        IStructureProperty GetIdProperty(Type structureType);
        IStructureProperty GetConcurrencyTokenProperty(Type structureType);
        IStructureProperty GetTimeStampProperty(Type structureType);

        IStructureProperty[] GetIndexableProperties(Type structureType, bool includeContainedStructureMembers);
		IStructureProperty[] GetIndexablePropertiesExcept(Type structureType, bool includeContainedStructureMembers, ICollection<string> nonIndexablePaths);
        IStructureProperty[] GetSpecificIndexableProperties(Type structureType, bool includeContainedStructureMembers, ICollection<string> indexablePaths);
        IStructureProperty[] GetContainedStructureProperties(Type structureType);
    }
}