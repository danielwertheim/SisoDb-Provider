using System;
using SisoDb.Structures.Schemas.Configuration;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureTypeFactory
    {
        IStructureTypeConfigurations Configurations { get; set; }

        IStructureType CreateFor(Type type);
    }
}