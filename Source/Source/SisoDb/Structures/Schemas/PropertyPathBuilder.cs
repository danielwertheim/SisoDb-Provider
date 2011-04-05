namespace SisoDb.Structures.Schemas
{
    public static class PropertyPathBuilder
    {
        public static string BuildPath(IStructureProperty property)
        {
            if (property.IsRootMember)
                return property.Name;

            var parentPath = BuildPath(property.Parent);

            return parentPath + "." + property.Name;            
        }
    }
}