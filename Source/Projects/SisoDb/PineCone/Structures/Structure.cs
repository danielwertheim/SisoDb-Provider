using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.EnsureThat;
using SisoDb.NCore;
using SisoDb.PineCone.Resources;

namespace SisoDb.PineCone.Structures
{
    [Serializable]
    public class Structure : IStructure, IEquatable<IStructure>
    {
        public IStructureId Id { get; private set; }

        public string Name { get; private set; }

        public string Data { get; set; }

        public IList<IStructureIndex> Indexes { get; private set; }

        public IList<IStructureIndex> Uniques { get; private set; }

        private Structure()
        {
            Indexes = Lists.Empty<IStructureIndex>();
            Uniques = Lists.Empty<IStructureIndex>();
        }

        public Structure(string name, IStructureId id, IStructureIndex[] indexes, string data = null)
        {
            Ensure.That(name, "name").IsNotNullOrWhiteSpace();
            Ensure.That(id, "id").IsNotNull();
            
            Name = name;
            Id = id;
            Data = data;
            Indexes = Lists.New(indexes);
            Uniques = Lists.New(indexes.Where(i => i.IsUnique).ToArray());

            if (Uniques.Count > 0)
            {
                var firstUniqueNotBeingUnique =
                    Uniques.FirstOrDefault(u => Indexes.Count(i => i.Path.Equals(u.Path)) > 1);
                if (firstUniqueNotBeingUnique != null)
                {
                    var idValue = Sys.StringConverter.AsString(firstUniqueNotBeingUnique.StructureId.Value);
                    var uniqueValue = Sys.StringConverter.AsString(firstUniqueNotBeingUnique.Value);
                    throw new PineConeException(
                        ExceptionMessages.Structure_DuplicateUniques.Inject(
                            Name,
                            idValue,
                            firstUniqueNotBeingUnique.Path,
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
                return (Id.GetHashCode() * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}