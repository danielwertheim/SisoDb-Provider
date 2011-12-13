using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EnsureThat;

namespace SisoDb
{
    public static class DbWriteOnceOpExtensions
    {
        public static T Insert<T>(this DbWriteOnceOp db, T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using(var uow = db.Instance.CreateUnitOfWork())
            {
                uow.Insert(item);
                uow.Commit();
            }

            return item;
        }

        public static void InsertJson<T>(this DbWriteOnceOp db, string json) where T : class
        {
            Ensure.That(json, "json").IsNotNullOrWhiteSpace();

            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.InsertJson<T>(json);
                uow.Commit();
            }
        }

        public static IList<T> InsertMany<T>(this DbWriteOnceOp db, IList<T> items) where T : class
        {
            Ensure.That(items, "items").HasItems();

            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.InsertMany(items);
                uow.Commit();
            }

            return items;
        }

        public static void InsertManyJson<T>(this DbWriteOnceOp db, IList<string> json) where T : class
        {
            Ensure.That(json, "json").HasItems();

            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.InsertManyJson<T>(json);
                uow.Commit();
            }
        }

        public static T Update<T>(this DbWriteOnceOp db, T item) where T : class
        {
            Ensure.That(item, "item").IsNotNull();

            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.Update(item);
                uow.Commit();
            }

            return item;
        }

        public static void DeleteById<T>(this DbWriteOnceOp db, object id) where T : class
        {
            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.DeleteById<T>(id);
                uow.Commit();
            }
        }

        public static void DeleteByIds<T>(this DbWriteOnceOp db, params object[] ids) where T : class
        {
            Ensure.That(ids, "ids").HasItems();

            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.DeleteByIds<T>(ids);
                uow.Commit();
            }
        }

        public static void DeleteByIdInterval<T>(this DbWriteOnceOp db, object idFrom, object idTo) where T : class
        {
            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.DeleteByIdInterval<T>(idFrom, idTo);
                uow.Commit();
            }
        }

        public static void DeleteByQuery<T>(this DbWriteOnceOp db, Expression<Func<T, bool>> expression) where T : class
        {
            using (var uow = db.Instance.CreateUnitOfWork())
            {
                uow.DeleteByQuery(expression);
                uow.Commit();
            }
        }
    }
}