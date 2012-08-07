using System;
using System.Collections.Generic;

namespace SisoDb.PineCone.Structures.Schemas.Configuration
{
    public interface IStructureTypeConfigurations
    {
        bool IsEmpty { get; }
        IEnumerable<IStructureTypeConfig> Items { get; }

        void Clear();
        void Configure(Type type, Action<IStructureTypeConfigurator> configure);
        void Configure<T>(Action<IStructureTypeConfigurator<T>> configure) where T : class;
        IStructureTypeConfig GetConfiguration(Type type);
        IStructureTypeConfig GetConfiguration<T>() where T : class;
    }
}