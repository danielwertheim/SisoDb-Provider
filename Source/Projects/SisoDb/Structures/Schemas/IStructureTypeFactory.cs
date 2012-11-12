using System;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureTypeFactory
    {
        Func<IStructureTypeConfig, IStructureTypeReflecter> ReflecterFn { get; set; }
        IStructureTypeConfigurations Configurations { get; set; }

        IStructureType CreateFor<T>() where T : class;
        IStructureType CreateFor(Type structureType);
    }
}