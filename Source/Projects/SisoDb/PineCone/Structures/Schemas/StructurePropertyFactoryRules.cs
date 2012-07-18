using System;
using System.Linq;

namespace SisoDb.PineCone.Structures.Schemas
{
    public class StructurePropertyFactoryRules
    {
        private static readonly string[] DefaultTextDataTypeConventions = new string[] { "Text", "Content", "Description" };

        public Func<string, bool> MemberNameIsForTextType { get; set; }

        public StructurePropertyFactoryRules()
        {
            MemberNameIsForTextType = OnMemberNameIsForTextType;
        }

        protected virtual bool OnMemberNameIsForTextType(string memberName)
        {
            return DefaultTextDataTypeConventions.Any(conv => conv.EndsWith(memberName, StringComparison.OrdinalIgnoreCase));
        }
    }
}