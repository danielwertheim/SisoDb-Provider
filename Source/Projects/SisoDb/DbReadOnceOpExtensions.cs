using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using SisoDb.Querying;

namespace SisoDb
{
    public static class DbReadOnceOpExtensions
    {
        public static int Count<T>(this DbReadOnceOp db) where T : class
        {
			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.Query<T>().Count();
            }
        }

        public static int Count<T>(this DbReadOnceOp db, Expression<Func<T, bool>> expression) where T : class
        {
            Ensure.That(expression, "expression").IsNotNull();

        	return db.Query<T>().Count(expression);
        }

        public static T GetById<T>(this DbReadOnceOp db, object id) where T : class
        {
			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.GetById<T>(id);
            }
        }

        public static TOut GetByIdAs<TContract, TOut>(this DbReadOnceOp db, object id)
            where TContract : class
            where TOut : class
        {
			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.GetByIdAs<TContract, TOut>(id);
            }
        }

        public static IList<T> GetByIds<T>(this DbReadOnceOp db, params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.GetByIds<T>(ids).ToList();
            }
        }

        public static IList<TOut> GetByIdsAs<TContract, TOut>(this DbReadOnceOp db, params object[] ids)
            where TContract : class
            where TOut : class
        {
            Ensure.That(ids, "ids").HasItems();

			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.GetByIdsAs<TContract, TOut>(ids).ToList();
            }
        }

        public static IList<T> GetByIdInterval<T>(this DbReadOnceOp db, object idFrom, object idTo) where T : class
        {
			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.GetByIdInterval<T>(idFrom, idTo).ToList();
            }
        }

        public static string GetByIdAsJson<T>(this DbReadOnceOp db, object id) where T : class
        {
			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.GetByIdAsJson<T>(id);
            }
        }

        public static IList<string> GetByIdsAsJson<T>(this DbReadOnceOp db, params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.GetByIdsAsJson<T>(ids).ToList();
            }
        }

        public static IList<T> NamedQuery<T>(this DbReadOnceOp db, INamedQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.Advanced.NamedQuery<T>(query).ToList();
            }
        }

        public static IList<TOut> NamedQueryAs<TContract, TOut>(this DbReadOnceOp db, INamedQuery query)
            where TContract : class
            where TOut : class
        {
            Ensure.That(query, "query").IsNotNull();

			using (var rs = db.Instance.CreateReadSession())
            {
				return rs.Advanced.NamedQueryAs<TContract, TOut>(query).ToList();
            }
        }

        public static IList<string> NamedQueryAsJson<T>(this DbReadOnceOp db, INamedQuery query) where T : class
        {
            Ensure.That(query, "query").IsNotNull();

            using (var rs = db.Instance.CreateReadSession())
            {
				return rs.Advanced.NamedQueryAsJson<T>(query).ToList();
            }
        }

        public static ISisoQueryable<T> Query<T>(this DbReadOnceOp db) where T : class
        {
			return new SisoReadOnceQueryable<T>(db.Instance.ProviderFactory.GetQueryBuilder<T>(db.Instance.StructureSchemas), () => db.Instance.CreateReadSession());
        }
    }
}