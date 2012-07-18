using System.Reflection;

namespace SisoDb.PineCone.Structures.Schemas
{
    public interface IStructurePropertyFactory
    {
        IStructureProperty CreateRootPropertyFrom(PropertyInfo propertyInfo);
        IStructureProperty CreateChildPropertyFrom(IStructureProperty parent, PropertyInfo propertyInfo);
    }
}