using System.Collections.Generic;
using SisoDb.EnsureThat;
using SisoDb.PineCone.Structures.Schemas.MemberAccessors;

namespace SisoDb.PineCone.Structures.Schemas
{
    public class StructureSchema : IStructureSchema
    {
        public IStructureType Type { get; private set; }

        public string Name
        {
            get { return Type.Name; }
        }

        public bool HasId
        {
            get { return IdAccessor != null; }
        }

        public bool HasConcurrencyToken
        {
            get { return ConcurrencyTokenAccessor != null; }
        }

        public bool HasTimeStamp
        {
            get { return TimeStampAccessor != null; }
        }

        public IIdAccessor IdAccessor { get; private set; }
        public IConcurrencyTokenAccessor ConcurrencyTokenAccessor { get; private set; }
        public ITimeStampAccessor TimeStampAccessor { get; private set; }
        public IList<IIndexAccessor> IndexAccessors { get; private set; }

        public StructureSchema(IStructureType type, IIdAccessor idAccessor = null, IConcurrencyTokenAccessor concurrencyTokenAccessor = null, ITimeStampAccessor timeStampAccessor = null, ICollection<IIndexAccessor> indexAccessors = null)
        {
            Ensure.That(type, "type").IsNotNull();

            Type = type;
            IdAccessor = idAccessor;
            ConcurrencyTokenAccessor = concurrencyTokenAccessor;
            TimeStampAccessor = timeStampAccessor;

            IndexAccessors = indexAccessors != null 
                ? new List<IIndexAccessor>(indexAccessors)
                : new List<IIndexAccessor>();
        }
    }
}