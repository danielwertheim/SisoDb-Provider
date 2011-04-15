namespace SisoDb.Structures.Schemas
{
    public static class PropertyPathBuilder
    {
        public static string BuildPath(IStructureProperty property)
        {
            if (property.IsRootMember)
                return property.Name;

            return BuildPath(property.Parent) + "." + property.Name;            
        }
    }
}