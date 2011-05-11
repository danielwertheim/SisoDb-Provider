using System;
using System.Collections.Generic;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.TestModel
{
    internal static class CustomerFactory
    {
        internal static IList<Customer> CreateCustomers(int numOfCustomers)
        {
            var customers = new List<Customer>();

            for (var c = 0; c < numOfCustomers; c++)
            {
                var n = c + 1;
                customers.Add(new Customer
                {
                    CustomerNo = n,
                    Firstname = "Daniel",
                    Lastname = "Wertheim",
                    ShoppingIndex = ShoppingIndexes.Level1,
                    CustomerSince = DateTime.Now,
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

            return customers;
        }
    }
}