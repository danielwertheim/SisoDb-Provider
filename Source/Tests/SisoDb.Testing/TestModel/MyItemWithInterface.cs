using System;
using PineCone.Annotations;

namespace SisoDb.Testing.TestModel
{
    public class MyItemWithInterface : IMyItemInterface
    {
        public int MyItemInt { get; set; }

        public Guid StructureId { get; set; }
        
        public int MyItemInterfaceInt { get; set; }

        [Unique(UniqueModes.PerType)]
        public int MyItemInterfaceUniqueInt { get; set; }
    }
}