using System;
using SisoDb.Annotations;

namespace SisoDbLab.Model
{
    [Serializable]
    public class OrderLine
    {
        [Unique(UniqueModes.PerInstance)]
        public string ProductNo { get; set; }

        public int Quantity { get; set; }
    }
}