using System;

namespace SisoDb.SampleApp.Model
{
    [Serializable]
    public class Customer
    {
        public Guid Id { get; set; }

        public int CustomerNo { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public decimal Score { get; set; }

        public ShoppingIndexes ShoppingIndex { get; set; }

        public DateTime CustomerSince { get; set; }

        public Address BillingAddress { get; set; }

        public Address DeliveryAddress { get; set; }

        public bool IsActive { get; set; }

        public Customer()
        {
            ShoppingIndex = ShoppingIndexes.Level0;
            BillingAddress = new Address();
            DeliveryAddress = new Address();
            IsActive = true;
            Score = 55.33m;
        }
    }
}