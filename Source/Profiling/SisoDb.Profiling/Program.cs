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

			//ProfilingInserts(db, 10000, 5);

			//InsertCustomers(1, 1000, db);
			//ProfilingQueries(db, GetAllCustomers);
			//ProfilingQueries(db, GetAllCustomersAsJson);
			//ProfilingQueries(db, GetAllCustomersViaIndexesTable);
			//ProfilingQueries(db, GetAllCustomersAsJsonViaIndexesTable);

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

        private static void ProfilingQueries<T>(ISisoDatabase database, Func<ISisoDatabase, IList<T>> queryAction)
        {
            var stopWatch = new Stopwatch();
            
            stopWatch.Start();
            var customers = queryAction(database);
            stopWatch.Stop();
            
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

        private static void InsertCustomers(IList<Customer> customers, ISisoDatabase database)
        {
            using (var unitOfWork = database.CreateUnitOfWork())
            {
                unitOfWork.InsertMany(customers);
                unitOfWork.Commit();
            }
        }

        private static IList<Customer> GetAllCustomers(ISisoDatabase database)
        {
        	return database.ReadOnce().Query<Customer>().ToList();
        }

        private static IList<Customer> GetAllCustomersViaIndexesTable(ISisoDatabase database)
        {
			return database.ReadOnce().Query<Customer>().Where(c => c.StructureId == c.StructureId).ToList();
        }

        private static IList<string> GetAllCustomersAsJsonViaIndexesTable(ISisoDatabase database)
        {
			return database.ReadOnce().Query<Customer>().Where(c => c.StructureId == c.StructureId).ToListOfJson();
        }

        private static IList<string> GetAllCustomersAsJson(ISisoDatabase database)
        {
			return database.ReadOnce().Query<Customer>().ToListOfJson();
        }
    }
}
