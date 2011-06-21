using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Core;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    public class StructureSchema : IStructureSchema
    {
        public const string IdMemberName = "SisoId";

        private readonly string _delimitedIndexAccessorNames;
        private readonly string _delimitedUniqueIndexAccessorNames;

        public string Name { get; private set; }

        public string Hash { get; private set; }

        public IIdAccessor IdAccessor { get; private set; }

        public IList<IIndexAccessor> IndexAccessors { get; private set; }

        public IList<IIndexAccessor> UniqueIndexAccessors { get; private set; }
        
        public StructureSchema(string name, string hash, IIdAccessor idAccessor, IEnumerable<IIndexAccessor> indexAccessors = null)
        {
            Name = name.AssertNotNullOrWhiteSpace("name");
            Hash = hash.AssertNotNullOrWhiteSpace("hash");
            IdAccessor = idAccessor.AssertNotNull("idAccessor");

            IndexAccessors = indexAccessors != null ? new List<IIndexAccessor>(indexAccessors) 
                : new List<IIndexAccessor>();

            UniqueIndexAccessors = indexAccessors != null ? new List<IIndexAccessor>(indexAccessors.Where(iac => iac.IsUnique))
                : new List<IIndexAccessor>();

            _delimitedIndexAccessorNames = string.Join(
                ",",
                IndexAccessors
                    .Select(iac => "[{0}]".Inject(iac.Name))
                    .ToArray());

            _delimitedUniqueIndexAccessorNames = string.Join(
                ",",
                IndexAccessors
                    .Where(iac => iac.IsUnique)
                    .Select(iac => "[{0}]".Inject(iac.Name))
                    .ToArray());
        }
    }
}