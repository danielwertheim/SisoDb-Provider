using System;
using SisoDb.Annotations;

namespace SisoDbLab.Model
{
    [Serializable]
    public class Customer
    {
        public Guid Id { get; set; }

        [Unique(UniqueModes.PerType)]
        public string PersonalNo { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public ShoppingIndexes ShoppingIndex { get; set; }

        public DateTime CustomerSince { get; set; }

        public Address BillingAddress { get; set; }

        public Address DeliveryAddress { get; set; }

        public Customer()
        {
            ShoppingIndex = ShoppingIndexes.Level0;
            BillingAddress = new Address();
            DeliveryAddress = new Address();
        }
    }
}