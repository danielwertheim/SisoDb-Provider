using System.Collections.Generic;
using SisoDb.Structures.Schemas.MemberAccessors;

namespace SisoDb.Structures.Schemas
{
    public interface IStructureSchema
    {
        string Name { get; }

        string Hash { get; }

        IIdAccessor IdAccessor { get; }

        IList<IIndexAccessor> IndexAccessors { get; }

        IList<IIndexAccessor> UniqueIndexAccessors { get; }
    }
}