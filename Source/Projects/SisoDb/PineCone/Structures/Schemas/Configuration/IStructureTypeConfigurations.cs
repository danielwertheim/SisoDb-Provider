using System;
using System.Collections.Generic;

namespace PineCone.Structures.Schemas.Configuration
{
    public interface IStructureTypeConfigurations
    {
        bool IsEmpty { get; }

        IEnumerable<IStructureTypeConfig> Items { get; }

        void Clear();

        IStructureTypeConfig GetConfiguration(Type type);

        IStructureTypeConfig GetConfiguration<T>() where T : class;

        IStructureTypeConfig NewForType(Type type);

        IStructureTypeConfig<T> NewForType<T>() where T : class;
    }
}