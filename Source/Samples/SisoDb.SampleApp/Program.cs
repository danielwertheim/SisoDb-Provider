using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SisoDb.Dac;
using SisoDb.Querying;
using SisoDb.SampleApp.Model;
using SisoDb.Sql2005;
using SisoDb.Sql2008;
using SisoDb.Sql2012;
using SisoDb.SqlCe4;

namespace SisoDb.SampleApp
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi. Goto the Sample-app and open Program.cs and App.config and ensure that you are satisfied with the connection string.");
            Console.ReadKey();
            return;

            //********* SQL2005 ***********
            //var db = "SisoDb.Sql2005".CreateSql2005Db();
            //********* SQL2008 ***********
            //var db = "SisoDb.Sql2008".CreateSql2008Db();
            //********* SQL2012 ***********
            //var db = "SisoDb.Sql2012".CreateSql2012Db();
            //********* SQLCE4 ***********
            //var db = "SisoDb.SqlCe4".CreateSqlCe4Db();
            //********************************************

            //db.EnsureNewDatabase();
            
            //Some tweaks
            //db.Settings.AllowDynamicSchemaCreation = false;
            //db.Settings.AllowDynamicSchemaUpdates = false;

            //To get rid of warm up in tests as it first syncs schemas etc
            //db.UpsertStructureSet<Customer>();

            //InsertCustomers(1, 10000, db);

            //ProfilingInserts(db, 1000, 5);
            //ProfilingQueries(() => FirstOrDefault(db, 500, 550));
            //ProfilingQueries(() => SingleOrDefault(db, 500, 550));
            //ProfilingQueries(() => GetAllCustomers(db));
            //ProfilingQueries(() => GetAllCustomersAsJson(db));
            //ProfilingQueries(() => GetCustomersViaIndexesTable(db, 500, 550));
            //ProfilingQueries(() => GetCustomersAsJsonViaIndexesTable(db, 500, 550));

            //UpsertSp(db, 500, 550);
            //ProfilingQueries(() => GetCustomersViaSpExp(db, 500, 550));
            //ProfilingQueries(() => GetCustomersViaSpRaw(db, 500, 550));

            //ProfilingUpdateMany(db, 500, 550);

            //Console.WriteLine("---- Done ----");
            //Console.ReadKey();
        }

        private static void ProfilingUpdateMany(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            using (var session = database.BeginSession())
            {
                session.UpdateMany<Customer>(
                    c => c.CustomerNo >= customerNoFrom && c.CustomerNo <= customerNoTo,
                    customer => { customer.Firstname += "Udated"; });
            }

            stopWatch.Stop();
            Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

            using (var rs = database.BeginSession())
            {
                var rowCount = rs.Query<Customer>().Count();

                Console.WriteLine("Total rows = {0}", rowCount);
            }
        }

        private static void ProfilingInserts(ISisoDatabase database, int numOfCustomers, int numOfItterations)
        {
            var stopWatch = new Stopwatch();

            for (var c = 0; c < numOfItterations; c++)
            {
                var customers = CustomerFactory.CreateCustomers(numOfCustomers);
                stopWatch.Start();
                InsertCustomers(customers, database);
                stopWatch.Stop();

                Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

                stopWatch.Reset();
            }

            using (var rs = database.BeginSession())
            {
                var rowCount = rs.Query<Customer>().Count();

                Console.WriteLine("Total rows = {0}", rowCount);
            }
        }

        private static void ProfilingQueries(Func<int> queryAction)
        {
            for (var c = 0; c < 2; c++)
            {
                var stopWatch = new Stopwatch();

                stopWatch.Start();
                var customersCount = queryAction();
                stopWatch.Stop();

                Console.WriteLine("customers.Count() = {0}", customersCount);
                Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);
            }
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
            using (var unitOfWork = database.BeginSession())
            {
                unitOfWork.InsertMany(customers);
            }
        }

        private static int GetAllCustomers(ISisoDatabase database)
        {
            using (var session = database.BeginSession())
            {
                return session.Query<Customer>().ToEnumerable().Count();
            }
        }

        private static int GetAllCustomersAsJson(ISisoDatabase database)
        {
            return database.UseOnceTo().Query<Customer>().ToEnumerableOfJson().Count();
        }

        private static int GetCustomersViaIndexesTable(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            using (var session = database.BeginSession())
            {
                return session.Query<Customer>().Where(c => c.CustomerNo >= customerNoFrom && c.CustomerNo <= customerNoTo && c.DeliveryAddress.Street == "The delivery street #544").ToEnumerable().Count();
            }
        }

        private static void UpsertSp(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            using (var session = database.BeginSession())
            {
                session.Advanced.UpsertNamedQuery<Customer>("CustomersViaSP", qb => qb.Where(c =>
                    c.CustomerNo >= customerNoFrom
                    && c.CustomerNo <= customerNoTo
                    && c.DeliveryAddress.Street == "The delivery street #544"));
            }
        }

        private static int GetCustomersViaSpRaw(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            using (var session = database.BeginSession())
            {
                var q = new NamedQuery("CustomersViaSP");
                q.Add(new DacParameter("p0", customerNoFrom));
                q.Add(new DacParameter("p1", customerNoTo));
                q.Add(new DacParameter("p2", "The delivery street #544"));

                return session.Advanced.NamedQuery<Customer>(q).ToArray().Length;
            }
        }
        
        private static int GetCustomersViaSpExp(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            using (var session = database.BeginSession())
            {
                return session.Advanced.NamedQuery<Customer>("CustomersViaSP", c =>
                    c.CustomerNo >= customerNoFrom
                    && c.CustomerNo <= customerNoTo
                    && c.DeliveryAddress.Street == "The delivery street #544").ToArray().Length;
            }
        }
        
        private static int GetCustomersAsJsonViaIndexesTable(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            using (var session = database.BeginSession())
            {
                return session.Query<Customer>().Where(c => c.CustomerNo >= customerNoFrom && c.CustomerNo <= customerNoTo && c.DeliveryAddress.Street == "The delivery street #544").ToEnumerableOfJson().Count();
            }
        }

        private static int FirstOrDefault(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            using (var session = database.BeginSession())
            {
                return session.Query<Customer>().Where(c => c.CustomerNo >= customerNoFrom && c.CustomerNo <= customerNoTo && c.DeliveryAddress.Street == "The delivery street #544").FirstOrDefault() == null ? 0 : 1;
            }
        }

        private static int SingleOrDefault(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
            using (var session = database.BeginSession())
            {
                return session.Query<Customer>().Where(c => c.CustomerNo >= customerNoFrom && c.CustomerNo <= customerNoTo && c.DeliveryAddress.Street == "The delivery street #544").SingleOrDefault() == null ? 0 : 1;
            }
        }
    }
}
