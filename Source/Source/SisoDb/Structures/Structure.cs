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

        public string TypeName { get; private set; }

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

            TypeName = typeName;
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
                    var idValue = SisoDbEnvironment.StringConverter.AsString(firstUniqueNotBeingUnique.StructureId.Value);
                    var uniqueValue = SisoDbEnvironment.StringConverter.AsString(firstUniqueNotBeingUnique.Value);
                    throw new SisoDbException(
                        ExceptionMessages.Structure_DuplicateUniques.Inject(
                            TypeName,
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
            return Equals(other.Id, Id) && Equals(other.TypeName, TypeName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Id != null ? Id.GetHashCode() : 0) * 397) ^ (TypeName != null ? TypeName.GetHashCode() : 0);
            }
        }
    }
}