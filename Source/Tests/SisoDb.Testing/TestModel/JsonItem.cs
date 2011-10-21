using System;

namespace SisoDb.Testing.TestModel
{
    public class JsonItem
    {
        public Guid StructureId { get; set; }

        public string String1 { get; set; }

        public int Int1 { get; set; }

        public decimal Decimal1 { get; set; }

        public DateTime? DateTime1 { get; set; }

        public int[] Ints { get; set; }
    }
}