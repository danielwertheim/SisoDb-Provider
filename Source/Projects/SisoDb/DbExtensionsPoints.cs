using System;
using System.Diagnostics;

namespace SisoDb
{
    public static class DbExtensionsPoints
    {
        /// <summary>
        /// Use when you want to execute a single Fetch against the <see cref="ISisoDatabase"/>
        /// via an <see cref="IQueryEngine"/>.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// <remarks>If you need to do multiple queries, use <see cref="ISisoDatabase.CreateQueryEngine"/> instead.</remarks>
        [DebuggerStepThrough]
        public static DbQueryExtensionPoint ReadOnce(this ISisoDatabase db)
        {
            return new DbQueryExtensionPoint(db);
        }

        /// <summary>
        /// Use when you want to execute a single Insert, Update or Delete against 
        /// the <see cref="ISisoDatabase"/> via an <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        /// <remarks>If you need to do multiple operations in the <see cref="IUnitOfWork"/>,
        /// use <see cref="ISisoDatabase.CreateUnitOfWork"/> instead.</remarks>
        [DebuggerStepThrough]
        public static DbUoWExtensionPoint WriteOnce(this ISisoDatabase db)
        {
            return new DbUoWExtensionPoint(db);
        }

        /// <summary>
        /// Simplifies usage of <see cref="IUnitOfWork"/>.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="consumer"></param>
        [DebuggerStepThrough]
        public static void WithUnitOfWork(this ISisoDatabase db, Action<IUnitOfWork> consumer)
        {
            using (var uow = db.CreateUnitOfWork())
            {
                consumer.Invoke(uow);
            }
        }

        /// <summary>
        /// Simplifies usage of <see cref="IQueryEngine"/>.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="consumer"></param>
        [DebuggerStepThrough]
        public static void WithQueryEngine(this ISisoDatabase db, Action<IQueryEngine> consumer)
        {
            using (var qe = db.CreateQueryEngine())
            {
                consumer.Invoke(qe);
            }
        }
    }
}