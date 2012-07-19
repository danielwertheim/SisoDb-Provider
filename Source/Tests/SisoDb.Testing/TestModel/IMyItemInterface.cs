using System;
using SisoDb.PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    public interface IMyItemInterface
    {
        Guid StructureId { get; set; }

        int MyItemInterfaceInt { get; set; }

        [Unique(UniqueModes.PerType)]
        int MyItemInterfaceUniqueInt { get; set; }
    }
}