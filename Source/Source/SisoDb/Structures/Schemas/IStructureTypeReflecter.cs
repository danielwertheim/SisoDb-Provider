using System.Collections.Generic;
using System.Reflection;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureTypeReflecter
    {
        bool HasIdProperty(IReflect type);
        IProperty GetIdProperty(IReflect type);
        IEnumerable<IProperty> GetIndexableProperties(IReflect type, IEnumerable<string> nonIndexableNames = null);
        IEnumerable<PropertyInfo> GetSimpleIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexableNames = null);
        IEnumerable<PropertyInfo> GetComplexIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexableNames = null);
        IEnumerable<PropertyInfo> GetEnumerableIndexablePropertyInfos(IReflect type, IEnumerable<string> nonIndexableNames = null);
    }
}