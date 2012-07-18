using System;
using PineCone.Structures.Schemas.Configuration;

namespace PineCone.Structures.Schemas
{
    public interface IStructureTypeFactory
    {
        IStructureTypeReflecter Reflecter { get; }

        IStructureTypeConfigurations Configurations { get; }

        IStructureType CreateFor<T>() where T : class;

        IStructureType CreateFor(Type type);
    }
}