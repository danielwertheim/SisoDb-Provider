using System.Collections.Generic;
using NCore;

namespace SisoDb.SampleApp.Model
{
    internal static class CustomerFactory
    {
        private static int ItteratorCount = 0;

        internal static IList<Customer> CreateCustomers(int numOfCustomers)
        {
            var customers = new List<Customer>();

            for (var c = 0; c < numOfCustomers; c++)
            {
                var n = ItteratorCount + (c + 1);
                customers.Add(new Customer
                {
                    //StructureId = n.ToString(),
                    CustomerNo = n,
                    Firstname = "Daniel",
                    Lastname = "Wertheim",
                    ShoppingIndex = ShoppingIndexes.Level1,
                    CustomerSince = SysDateTime.Now,
                    BillingAddress =
                        {
                            Street = "The billing street " + n,
                            Zip = "12345",
                            City = "The billing City",
                            Country = "Sweden-billing",
                            AreaCode = 1000 + n
                        },
                    DeliveryAddress =
                        {
                            Street = "The delivery street #" + n,
                            Zip = "54321",
                            City = "The delivery City",
                            Country = "Sweden-delivery",
                            AreaCode = -1000 - n
                        }
                });
            }

            ItteratorCount += numOfCustomers;

            return customers;
        }
    }
}