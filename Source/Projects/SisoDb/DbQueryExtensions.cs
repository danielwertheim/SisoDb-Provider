using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using SisoDb.Querying;

namespace SisoDb
{
    public static class DbQueryExtensions
    {
        public static int Count<T>(this DbQueryExtensionPoint db) where T : class
        {
            using(var qe = db.Instance.CreateQueryEngine())
            {
                return qe.Count<T>();
            }
        }

        public static int Count<T>(this DbQueryExtensionPoint db, Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.Count<T>(expression);
            }
        }

        public static T GetById<T>(this DbQueryExtensionPoint db, ValueType id) where T : class
        {
            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetById<T>(id);
            }
        }

        public static TOut GetByIdAs<TContract, TOut>(this DbQueryExtensionPoint db, ValueType id)
            where TContract : class
            where TOut : class
        {
            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetByIdAs<TContract, TOut>(id);
            }
        }

        public static IList<T> GetByIds<T>(this DbQueryExtensionPoint db, params ValueType[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetByIds<T>(ids).ToList();
            }
        }

        public static IList<TOut> GetByIdsAs<TContract, TOut>(this DbQueryExtensionPoint db, params ValueType[] ids)
            where TContract : class
            where TOut : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetByIdsAs<TContract, TOut>(ids).ToList();
            }
        }

        public static IList<T> GetByIdInterval<T>(this DbQueryExtensionPoint db, ValueType idFrom, ValueType idTo) where T : class
        {
            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetByIdInterval<T>(idFrom, idTo).ToList();
            }
        }

        public static string GetByIdAsJson<T>(this DbQueryExtensionPoint db, ValueType id) where T : class
        {
            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetByIdAsJson<T>(id);
            }
        }

        public static IList<string> GetByIdsAsJson<T>(this DbQueryExtensionPoint db, params ValueType[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetByIdsAsJson<T>(ids).ToList();
            }
        }

        public static IList<T> GetAll<T>(this DbQueryExtensionPoint db) where T : class
        {
            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetAll<T>().ToList();
            }
        }

        public static IList<T> GetAll<T>(this DbQueryExtensionPoint db, Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            Ensure.That(commandInitializer, "commandInitializer").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetAll<T>(commandInitializer).ToList();
            }
        }

        public static IList<TOut> GetAllAs<TContract, TOut>(this DbQueryExtensionPoint db)
            where TContract : class
            where TOut : class
        {
            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetAllAs<TContract, TOut>().ToList();
            }
        }

        public static IList<TOut> GetAllAs<TContract, TOut>(this DbQueryExtensionPoint db, Action<IGetCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            Ensure.That(commandInitializer, "commandInitializer").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetAllAs<TContract, TOut>(commandInitializer).ToList();
            }
        }

        public static IList<string> GetAllAsJson<T>(this DbQueryExtensionPoint db) where T : class
        {
            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetAllAsJson<T>().ToList();
            }
        }

        public static IList<string> GetAllAsJson<T>(this DbQueryExtensionPoint db, Action<IGetCommandBuilder<T>> commandInitializer) where T : class
        {
            Ensure.That(commandInitializer, "commandInitializer").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.GetAllAsJson<T>(commandInitializer).ToList();
            }
        }

        public static IList<T> NamedQuery<T>(this DbQueryExtensionPoint db, INamedQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.Advanced.NamedQuery<T>(query).ToList();
            }
        }

        public static IList<TOut> NamedQueryAs<TContract, TOut>(this DbQueryExtensionPoint db, INamedQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.Advanced.NamedQueryAs<TContract, TOut>(query).ToList();
            }
        }

        public static IList<string> NamedQueryAsJson<T>(this DbQueryExtensionPoint db, INamedQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.Advanced.NamedQueryAsJson<T>(query).ToList();
            }
        }

        public static IList<T> Where<T>(this DbQueryExtensionPoint db, Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.Where<T>(expression).ToList();
            }
        }

        public static IList<TOut> WhereAs<TContract, TOut>(this DbQueryExtensionPoint db, Expression<Func<TContract, bool>> expression)
            where TContract : class
            where TOut : class
        {
            Ensure.That(expression, "expression").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.WhereAs<TContract, TOut>(expression).ToList();
            }
        }

        public static IList<string> WhereAsJson<T>(this DbQueryExtensionPoint db, Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.WhereAsJson<T>(expression).ToList();
            }
        }

        public static IList<T> Query<T>(this DbQueryExtensionPoint db, Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            Ensure.That(commandInitializer, "commandInitializer").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.Query<T>(commandInitializer).ToList();
            }
        }

        public static IList<TOut> QueryAs<TContract, TOut>(this DbQueryExtensionPoint db, Action<IQueryCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class
        {
            Ensure.That(commandInitializer, "commandInitializer").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.QueryAs<TContract, TOut>(commandInitializer).ToList();
            }
        }

        public static IList<string> QueryAsJson<T>(this DbQueryExtensionPoint db, Action<IQueryCommandBuilder<T>> commandInitializer) where T : class
        {
            Ensure.That(commandInitializer, "commandInitializer").IsNotNull();

            using (var qe = db.Instance.CreateQueryEngine())
            {
                return qe.QueryAsJson<T>(commandInitializer).ToList();
            }
        }
    }
}