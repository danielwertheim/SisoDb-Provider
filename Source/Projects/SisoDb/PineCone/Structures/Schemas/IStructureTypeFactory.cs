using System;
using SisoDb.PineCone.Structures.Schemas.Configuration;

namespace SisoDb.PineCone.Structures.Schemas
{
    public interface IStructureTypeFactory
    {
        IStructureTypeReflecter Reflecter { get; }

        IStructureTypeConfigurations Configurations { get; }

        IStructureType CreateFor<T>() where T : class;

        IStructureType CreateFor(Type type);
    }
}