using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SisoDb.Querying;

namespace SisoDb
{
    /// <summary>
    /// Used to query the database for structures.
    /// </summary>
    public interface IQueryEngine : IDisposable
    {
        /// <summary>
        /// Advances querying members.
        /// </summary>
        IAdvancedQueryEngine Advanced { get; }

        /// <summary>
        /// Issues a simple count for how many structures there are
        /// in the specified structure type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns>Number of structures.</returns>
        int Count<T>()
            where T : class;

        /// <summary>
        /// Issues a simple count for how many structures there are
        /// in the specified structure type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="expression"></param>
        /// <returns>Number of structures.</returns>
        int Count<T>(Expression<Func<T, bool>> expression)
            where T : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        /// <returns>Structure (<typeparamref name="T"/>) or Null</returns>
        T GetById<T>(object id)
            where T : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id. 
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="id"></param>
        /// <returns>Structure (<typeparamref name="TOut"/>) or null.</returns>
        TOut GetByIdAs<TContract, TOut>(object id)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetByIds<T>(params object[] ids)
            where T : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="TContract"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable of <typeparamref name="TOut"/>.</returns>
        IEnumerable<TOut> GetByIdsAs<TContract, TOut>(params object[] ids)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/>
        /// matching specified id interval.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="idFrom"></param>
        /// <param name="idTo"></param>
        /// <returns>IEnumerable of <typeparamref name="T"/>.</returns>
        /// /// <remarks>YOU COULD GET STRANGE RESULTS IF YOU HAVE SPECIFIED NON SEQUENTIAL GUIDS.</remarks>
        IEnumerable<T> GetByIdInterval<T>(object idFrom, object idTo)
            where T : class;

        /// <summary>
        /// Returns one single structure identified
        /// by an id, as Json. This is the most
        /// effective return type, since the Json
        /// is stored in the database, no deserialization
        /// will take place.  
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="id"></param>
        /// <returns>Json representation of (<typeparamref name="T"/>) or Null</returns>
        string GetByIdAsJson<T>(object id)
            where T : class;

        /// <summary>
        /// Returns Json representation for all structures for the defined structure <typeparamref name="T"/>
        /// matching passed ids.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="ids">Ids used for matching the structures to return.</param>
        /// <returns>IEnumerable Json representation of <typeparamref name="T"/>.</returns>
        IEnumerable<string> GetByIdsAsJson<T>(params object[] ids)
            where T : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns>All structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetAll<T>()
            where T : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/>.
        /// Lets you specify includes and sortorder via the CommandBuilder.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="commandInitializer"></param>
        /// <returns>All structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <typeparamref name="T"/>.</returns>
        IEnumerable<T> GetAll<T>(Action<IGetCommandBuilder<T>> commandInitializer)
            where T : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="TContract"/>
        /// as <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <returns>All structures (<typeparamref name="TOut"/>)
        /// or empty IEnumerable of <typeparamref name="TOut"/>.</returns>
        IEnumerable<TOut> GetAllAs<TContract, TOut>()
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="TContract"/>
        /// as <typeparamref name="TOut"/>.
        /// Lets you specify includes and sortorder via the CommandBuilder.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="commandInitializer"></param>
        /// <returns>All structures (<typeparamref name="TOut"/>)
        /// or empty IEnumerable of <typeparamref name="TOut"/>.</returns>
        IEnumerable<TOut> GetAllAs<TContract, TOut>(Action<IGetCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/> as Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns>Json representation of all structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        IEnumerable<string> GetAllAsJson<T>()
            where T : class;

        /// <summary>
        /// Returns all structures for the defined structure <typeparamref name="T"/> as Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place. 
        /// Lets you specify includes and sortorder via the CommandBuilder.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <returns>Json representation of all structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        IEnumerable<string> GetAllAsJson<T>(Action<IGetCommandBuilder<T>> commandInitializer)
            where T : class;

        /// <summary>
        /// Lets you invoke queries by passing in a simple where expression.
        /// <see cref="Query&lt;T&gt;"/> for more options like sorting and including referenced structures.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="expression"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
        IEnumerable<T> Where<T>(Expression<Func<T, bool>> expression)
            where T : class;

        /// <summary>
        /// Lets you invoke queries by passing in a simple where expression.
        /// <see cref="QueryAs&lt;TContract, TOut&gt;"/> for more options like sorting and including referenced structures.
        /// Returns structures for the defined structure <typeparamref name="TContract"/>
        /// deserialized as <typeparamref name="TOut"/>. 
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="expression"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
        IEnumerable<TOut> WhereAs<TContract, TOut>(Expression<Func<TContract, bool>> expression)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Lets you invoke queries by passing in a simple where expression.
        /// <see cref="QueryAsJson&lt;T&gt;"/> for more options like sorting and including referenced structures.
        /// Returns structures for the defined structure <typeparamref name="T"/> as Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns>Json representation of all structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        IEnumerable<string> WhereAsJson<T>(Expression<Func<T, bool>> expression)
            where T : class;

        /// <summary>
        /// Lets you invoke queries using the query command builder, which lets
        /// you specify where expressions, sorting expressions and include expressions.
        /// Returns structures for the defined structure <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="commandInitializer"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
        IEnumerable<T> Query<T>(Action<IQueryCommandBuilder<T>> commandInitializer)
            where T : class;

        /// <summary>
        /// Lets you invoke queries using the query command builder, which lets
        /// you specify where expressions, sorting expressions and include expressions.
        /// Returns structures for the defined structure <typeparamref name="TContract"/>
        /// deserialized as <typeparamref name="TOut"/>. 
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="commandInitializer"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
        IEnumerable<TOut> QueryAs<TContract, TOut>(Action<IQueryCommandBuilder<TContract>> commandInitializer)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Lets you invoke queries using the query command builder, which lets
        /// you specify where expressions, sorting expressions and include expressions.
        /// Returns structures for the defined structure <typeparamref name="T"/> as Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="commandInitializer"></param>
        /// <returns>Json representation of all structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        IEnumerable<string> QueryAsJson<T>(Action<IQueryCommandBuilder<T>> commandInitializer)
            where T : class;
    }
}