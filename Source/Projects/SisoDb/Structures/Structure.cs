using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Core;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    [Serializable]
    public class Structure : IStructure, IEquatable<IStructure>
    {
        public ISisoId Id { get; private set; }

        public string Name { get; private set; }

        public string Json { get; private set; }

        public ISet<IStructureIndex> Indexes { get; private set; }

        public ISet<IStructureIndex> Uniques { get; private set; }

        private Structure()
        {
            Indexes = new HashSet<IStructureIndex>();
            Uniques = new HashSet<IStructureIndex>();
        }

        public Structure(string name, ISisoId id, IEnumerable<IStructureIndex> indexes, string json)
        {
            name.AssertNotNullOrWhiteSpace("name");
            id.AssertNotNull("id");

            Name = name;
            Id = id;
            Json = json;

            Indexes = new HashSet<IStructureIndex>(indexes);
            Uniques = new HashSet<IStructureIndex>(indexes.Where(i => i.IsUnique));

            if (Uniques.Count > 0)
            {
                var firstUniqueNotBeingUnique =
                    Uniques.FirstOrDefault(u => indexes.Count(i => i.Name.Equals(u.Name)) > 1);
                if (firstUniqueNotBeingUnique != null)
                {
                    var idValue = SisoEnvironment.Formatting.StringConverter.AsString(firstUniqueNotBeingUnique.SisoId.Value);
                    var uniqueValue = SisoEnvironment.Formatting.StringConverter.AsString(firstUniqueNotBeingUnique.Value);
                    throw new SisoDbException(
                        ExceptionMessages.Structure_DuplicateUniques.Inject(
                            Name,
                            idValue,
                            firstUniqueNotBeingUnique.Name,
                            uniqueValue));
                }
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IStructure);
        }

        public bool Equals(IStructure other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Id, Id) && Equals(other.Name, Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}