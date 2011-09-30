using System.Collections.Generic;

namespace SisoDb.IntegrationTests.SqlProvider.UnitOfWork.TestModel
{
    internal static class CustomerDbHelperExtensions
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