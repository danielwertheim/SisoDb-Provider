using System;
using System.Collections.Generic;

namespace SisoDb.Structures.Schemas.Configuration
{
    public interface IStructureTypeConfigurations
    {
        bool IsEmpty { get; }

        IEnumerable<IStructureTypeConfig> Configurations { get; }

        IStructureTypeConfig GetConfiguration(Type type);

        IStructureTypeConfig GetConfiguration<T>() where T : class;

        IStructureTypeConfig NewForType(Type type);

        IStructureTypeConfig<T> NewForType<T>() where T : class;
    }
}