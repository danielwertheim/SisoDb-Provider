using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    internal class StructureSchema : IStructureSchema
    {
        public string Name { get; private set; }

        public string Hash { get; private set; }

        public IIdAccessor IdAccessor { get; private set; }

        public IList<IIndexAccessor> IndexAccessors { get; private set; }

        public IList<IIndexAccessor> UniqueIndexAccessors { get; private set; }
        
        public StructureSchema(string name, IIdAccessor idAccessor, IEnumerable<IIndexAccessor> indexAccessors = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException("name");

            if(idAccessor == null)
                throw new ArgumentNullException("idAccessor");

            Name = name;
            Hash = SisoDbEnvironment.HashService.GenerateHash(name);
            IdAccessor = idAccessor;
            IndexAccessors = indexAccessors != null ? new List<IIndexAccessor>(indexAccessors) 
                : new List<IIndexAccessor>();
            UniqueIndexAccessors = indexAccessors != null ? new List<IIndexAccessor>(indexAccessors.Where(iac => iac.IsUnique))
                : new List<IIndexAccessor>();
        }
    }
}