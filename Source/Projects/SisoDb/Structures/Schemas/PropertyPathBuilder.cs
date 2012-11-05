namespace SisoDb.Structures.Schemas
{
    public static class PropertyPathBuilder
    {
        public static string BuildPath(IStructureProperty property)
        {
            if (property.IsRootMember)
                return property.Name;

            return string.Concat(BuildPath(property.Parent), ".", property.Name);
        }

        public static string BuildPath(IStructureProperty parent, string name)
        {
            if (parent == null)
                return name;

            return string.Concat(parent.Path, ".", name);
        }
    }
}