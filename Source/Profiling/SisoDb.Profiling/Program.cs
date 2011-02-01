using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SisoDb.Profiling.Model;

namespace SisoDb.Profiling
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi. Goto the Profiling-app and open Program.cs and ensure that you are satisfied with the connection string.");
            Console.ReadKey();
            return;

            var cnInfo = new SisoConnectionInfo(@"sisodb:provider=Sql2008||plain:Data source=.;Initial catalog=SisoDbLab;Integrated security=SSPI;");

            var db = new SisoDatabase(cnInfo);
            //db.EnsureNewDatabase();

            //ProfilingInserts(db, 1000, 5);

            //InsertCustomers(1, 100000, db);
            //ProfilingQueries(db, GetAllCustomers);

            Console.WriteLine("---- Done ----");
            Console.ReadKey();
        }

        private static void ProfilingInserts(ISisoDatabase database, int numOfCustomers, int numOfItterations)
        {
            var durations = new List<TimeSpan>();
            var stopWatch = new Stopwatch();

            for (var c = 0; c < numOfItterations; c++)
            {
                var customers = CustomerFactory.CreateCustomers(numOfCustomers);
                stopWatch.Start();
                InsertCustomers(customers, database);
                stopWatch.Stop();

                durations.Add(stopWatch.Elapsed);

                Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

                stopWatch.Reset();
            }

            using (var unitOfWork = database.CreateUnitOfWork())
            {
                var rowCount = unitOfWork.Count<Customer>();

                Console.WriteLine("Total rows = {0}", rowCount);
            }
        }

        private static void ProfilingQueries<T>(ISisoDatabase database, Func<ISisoDatabase, IList<T>> queryAction)
        {
            var durations = new List<TimeSpan>();
            var stopWatch = new Stopwatch();
            
            stopWatch.Start();
            var customers = queryAction(database);
            stopWatch.Stop();

            durations.Add(stopWatch.Elapsed);

            Console.WriteLine("customers.Count() = {0}", customers.Count());
            Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);
        }

        private static void InsertCustomers(int numOfItterations, int numOfCustomers, ISisoDatabase database)
        {
            for (var c = 0; c < numOfItterations; c++)
            {
                var customers = CustomerFactory.CreateCustomers(numOfCustomers);
                InsertCustomers(customers, database);
            }
        }

        private static void InsertCustomers(IEnumerable<Customer> customers, ISisoDatabase database)
        {
            using (var unitOfWork = database.CreateUnitOfWork())
            {
                unitOfWork.InsertMany(customers);
                unitOfWork.Commit();
            }
        }

        private static IList<Customer> GetAllCustomers(ISisoDatabase database)
        {
            using (var unitOfWork = database.CreateUnitOfWork())
            {
                return unitOfWork.GetAll<Customer>().ToList();
            }
        }

        private static IList<string> GetAllCustomersAsJson(ISisoDatabase database)
        {
            using (var unitOfWork = database.CreateUnitOfWork())
            {
                return unitOfWork.GetAllAsJson<Customer>().ToList();
            }
        }
    }
}
