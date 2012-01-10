using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SisoDb.Profiling.Model;
using SisoDb.Sql2008;
using SisoDb.SqlCe4;

namespace SisoDb.Profiling
{
    public class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("Hi. Goto the Profiling-app and open Program.cs and ensure that you are satisfied with the connection string.");
			Console.ReadKey();
			return;

            //********* SQL2008 ***********
			//var cnInfo = new Sql2008ConnectionInfo(@"sisodb:provider=Sql2008||plain:Data source=.\sqlexpress;initial catalog=SisoDb.Profiling;integrated security=SSPI;MultipleActiveResultSets=true;");
			//var db = new Sql2008DbFactory().CreateDatabase(cnInfo);

            //********* SQLCE4 ***********
			//var cnInfo = new SqlCe4ConnectionInfo(@"sisodb:provider=SqlCe4||plain:Data source=D:\Temp\SisoDb.Profiling.sdf;");
			//var db = new SqlCe4DbFactory().CreateDatabase(cnInfo);

			//db.EnsureNewDatabase();

			//ProfilingInserts(db, 1000, 5);

			//InsertCustomers(1, 100000, db);
			//ProfilingQueries(() => GetAllCustomers(db));
			//ProfilingQueries(() => GetAllCustomersAsJson(db));
			//ProfilingQueries(() => GetCustomersViaIndexesTable(db, 500, 550));
			//ProfilingQueries(() => GetCustomersAsJsonViaIndexesTable(db, 500, 550));

			//ProfilingUpdateStructureSet(db);

			//Console.WriteLine("---- Done ----");
			//Console.ReadKey();
        }

        private static void ProfilingUpdateStructureSet(ISisoDatabase database)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
        	using (var uow = database.CreateUnitOfWork())
        	{
        		if(uow.UpdateMany<Customer>(customer => UpdateManyModifierStatus.Keep))
					uow.Commit();
        	}

            stopWatch.Stop();
            Console.WriteLine("TotalSeconds = {0}", stopWatch.Elapsed.TotalSeconds);

            using (var rs = database.CreateQueryEngine())
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

            using (var rs = database.CreateQueryEngine())
            {
                var rowCount = rs.Query<Customer>().Count();

                Console.WriteLine("Total rows = {0}", rowCount);
            }
        }

		private static void ProfilingQueries(Func<int> queryAction)
        {
            var stopWatch = new Stopwatch();
            
            stopWatch.Start();
            var customersCount = queryAction();
            stopWatch.Stop();
            
            Console.WriteLine("customers.Count() = {0}", customersCount);
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

        private static void InsertCustomers(IList<Customer> customers, ISisoDatabase database)
        {
            using (var unitOfWork = database.CreateUnitOfWork())
            {
                unitOfWork.InsertMany(customers);
                unitOfWork.Commit();
            }
        }

        private static int GetAllCustomers(ISisoDatabase database)
        {
			using(var qe = database.CreateQueryEngine())
			{
				return qe.Query<Customer>().ToEnumerable().Count();
			}
        }

    	private static int GetAllCustomersAsJson(ISisoDatabase database)
    	{
    		return database.ReadOnce().Query<Customer>().ToEnumerableOfJson().Count();
    	}

		private static int GetCustomersViaIndexesTable(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
			using (var qe = database.CreateQueryEngine())
			{
				return qe.Query<Customer>().Where(c => c.CustomerNo >= customerNoFrom && c.CustomerNo <= customerNoTo).ToEnumerable().Count();
			}
        }

		private static int GetCustomersAsJsonViaIndexesTable(ISisoDatabase database, int customerNoFrom, int customerNoTo)
        {
			using (var qe = database.CreateQueryEngine())
			{
				return qe.Query<Customer>().Where(c => c.CustomerNo >= customerNoFrom && c.CustomerNo <= customerNoTo).ToEnumerableOfJson().Count();
			}
        }
    }
}
