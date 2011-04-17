using System;
using System.Collections.Generic;

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

        public IEnumerable<IStructureTypeConfig> Configurations
        {
            get { return _configurations.Values; }
        }

        public StructureTypeConfigurations()
        {
            _configurations = new Dictionary<Type, IStructureTypeConfig>();
        }

        public IStructureTypeConfig GetConfiguration(Type type)
        {
            return _configurations.ContainsKey(type)
                       ? _configurations[type]
                       : null;
        }

        public IStructureTypeConfig GetConfiguration<T>() where T : class
        {
            return GetConfiguration(StructureTypeConfig<T>.TypeOfT);
        }

        public IStructureTypeConfig NewForType(Type type)
        {
            var config = new StructureTypeConfig(type);
            
            _configurations.Add(config.Type, config);

            return config;
        }

        public IStructureTypeConfig<T> NewForType<T>() where T:class 
        {
            var config = new StructureTypeConfig<T>();

            _configurations.Add(config.Type, config);

            return config;
        }
    }
}