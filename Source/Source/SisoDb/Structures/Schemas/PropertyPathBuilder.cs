namespace SisoDb.Structures.Schemas
{
    internal static class PropertyPathBuilder
    {
        internal static string BuildPath(IProperty property)
        {
            if (property.Level == 0)
                return property.Name;

            var parentPath = BuildPath(property.Parent);

            return parentPath + "." + property.Name;            
        }
    }
}