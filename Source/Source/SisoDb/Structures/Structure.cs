using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Resources;

namespace SisoDb.Structures
{
    [Serializable]
    public class Structure : IStructure, IEquatable<IStructure>
    {
        public IStructureId Id { get; private set; }

        //public byte[] Version { get; private set; }

        public string Name { get; private set; }

        public string Json { get; private set; }

        public ISet<IStructureIndex> Indexes { get; private set; }

        public ISet<IStructureIndex> Uniques { get; private set; }

        private Structure()
        {
            Indexes = new HashSet<IStructureIndex>();
            Uniques = new HashSet<IStructureIndex>();
        }

        public Structure(string typeName, IStructureId id, IEnumerable<IStructureIndex> indexes, string json)
        {
            if (typeName.IsNullOrWhiteSpace())
                throw new ArgumentNullException("typeName");

            if (id == null)
                throw new ArgumentNullException("id");

            Name = typeName;
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
                    var idValue = SisoDbEnvironment.Formatting.StringConverter.AsString(firstUniqueNotBeingUnique.StructureId.Value);
                    var uniqueValue = SisoDbEnvironment.Formatting.StringConverter.AsString(firstUniqueNotBeingUnique.Value);
                    throw new SisoDbException(
                        ExceptionMessages.Structure_DuplicateUniques.Inject(
                            Name,
                            idValue,
                            firstUniqueNotBeingUnique.Name,
                            (string)uniqueValue));
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