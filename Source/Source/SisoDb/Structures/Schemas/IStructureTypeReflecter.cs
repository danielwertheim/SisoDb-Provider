using System.Collections.Generic;
using System.Reflection;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureTypeReflecter
    {
        bool HasIdProperty(IReflect type);
        IStructureProperty GetIdProperty(IReflect type);
        IEnumerable<IStructureProperty> GetIndexableProperties(IReflect type, IEnumerable<string> nonIndexableNames = null);
    }
}