using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Annotations;

namespace SisoDbLab.Model
{
    [Serializable]
    public class Order
    {
        public int SisoId { get; set; }

        public Guid CustomerId { get; set; }

        [Unique(UniqueModes.PerType)]
        public string OrderNo { get; set; }

        public IList<OrderLine> Lines { get; set; }

        public Order()
        {
            Lines = new List<OrderLine>();
        }

        public void AddNewProduct(string productNo, int quantity)
        {
            var line = Lines.SingleOrDefault(l => l.ProductNo.Equals(productNo)) ?? new OrderLine{ProductNo = productNo};

            Lines.Add(line);

            line.Quantity += quantity;
        }
    }
}