using System.Collections.Generic;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008Provider.UnitOfWork.TestModel
{
    internal static class CustomerDbExtensions
    {
        internal static IList<Customer> CreateCustomers(this ISisoDatabase db, int numOfCustomers)
        {
            var customers = CustomerFactory.CreateCustomers(numOfCustomers);

            using (var unitOfWork = db.CreateUnitOfWork())
            {
                unitOfWork.InsertMany(customers);
                unitOfWork.Commit();
            }

            return customers;
        }
    }
}