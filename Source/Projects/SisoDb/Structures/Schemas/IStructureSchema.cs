using System.Collections.Generic;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchema
    {
		IStructureType Type { get; }
        string Name { get; }
        bool HasId { get; }
        bool HasConcurrencyToken { get; }
        bool HasTimeStamp { get; }
        IIdAccessor IdAccessor { get; }
        IConcurrencyTokenAccessor ConcurrencyTokenAccessor { get; }
        ITimeStampAccessor TimeStampAccessor { get; }
        IList<IIndexAccessor> IndexAccessors { get; }
    }
}