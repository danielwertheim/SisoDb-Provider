using System;
using System.Collections.Generic;
using SisoDb.EnsureThat;

namespace SisoDb.Structures.Schemas.Configuration
{
    [Serializable]
    public class StructureTypeConfigurations : IStructureTypeConfigurations
    {
        private readonly Dictionary<Type, IStructureTypeConfig> _configurations;

        public bool IsEmpty
        {
            get { return _configurations.Count < 1; }
        }

        public IEnumerable<IStructureTypeConfig> Items
        {
            get { return _configurations.Values; }
        }

        public StructureTypeConfigurations()
        {
            _configurations = new Dictionary<Type, IStructureTypeConfig>();
        }

        public virtual void Clear()
        {
            _configurations.Clear();
        }

        public virtual IStructureTypeConfig GetConfiguration(Type type)
        {
            return _configurations.ContainsKey(type)
                ? _configurations[type]
                : new StructureTypeConfig(type);
        }

        public virtual IStructureTypeConfig GetConfiguration<T>() where T : class
        {
            return GetConfiguration(typeof(T));
        }

        public virtual void Configure(Type type, Action<IStructureTypeConfigurator> configure)
        {
            Ensure.That(type, "type").IsNotNull();
            Ensure.That(configure, "configure").IsNotNull();

            var config = GetConfiguration(type);
            var configurator = new StructureTypeConfigurator(config);
            configure(configurator);

            _configurations[configurator.Config.Type] = configurator.Config;
        }

        public virtual void Configure<T>(Action<IStructureTypeConfigurator<T>> configure) where T : class 
        {
            Ensure.That(configure, "configure").IsNotNull();

            var config = GetConfiguration<T>();
            var configurator = new StructureTypeConfigurator<T>(config);
            configure(configurator);

            _configurations[configurator.Config.Type] = configurator.Config;
        }
    }
}