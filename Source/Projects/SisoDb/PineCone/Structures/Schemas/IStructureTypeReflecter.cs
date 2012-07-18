using System;
using System.Collections.Generic;

namespace PineCone.Structures.Schemas
{
    /// <summary>
    /// Responsible for identifying the Properties that should be used as
    /// StructureId, TimeStamp, ConcurrencyToken and plain indexes.
    /// </summary>
    public interface IStructureTypeReflecter
    {
        IStructurePropertyFactory PropertyFactory { get; set; }

        bool HasIdProperty(Type type);

        bool HasConcurrencyTokenProperty(Type type);

        bool HasTimeStampProperty(Type type);

        IStructureProperty GetIdProperty(Type type);

        IStructureProperty GetConcurrencyTokenProperty(Type type);

        IStructureProperty GetTimeStampProperty(Type type);

		IStructureProperty[] GetIndexableProperties(Type type);
						  
		IStructureProperty[] GetIndexablePropertiesExcept(Type type, ICollection<string> nonIndexablePaths);
						  
		IStructureProperty[] GetSpecificIndexableProperties(Type type, ICollection<string> indexablePaths);
    }
}