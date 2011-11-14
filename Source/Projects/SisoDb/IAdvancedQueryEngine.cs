using System.Collections.Generic;
using SisoDb.Querying;

namespace SisoDb
{
    /// <summary>
    /// Used to execute some more advances query operations against the database.
    /// </summary>
    public interface IAdvancedQueryEngine
    {
        /// <summary>
        /// Lets you invoke stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="T"/>.</returns>
        IEnumerable<T> NamedQuery<T>(INamedQuery query)
            where T : class;

        /// <summary>
        /// Lets you invoke stored procedures that returns Json,
        /// which will get deserialized to <typeparamref name="TOut"/>.
        /// </summary>
        /// <typeparam name="TContract">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <typeparam name="TOut">
        /// Determines the type you want your structure deserialized to and returned as.</typeparam>
        /// <param name="query"></param>
        /// <returns>Empty or populated IEnumerable of <typeparamref name="TOut"/>.</returns>
        IEnumerable<TOut> NamedQueryAs<TContract, TOut>(INamedQuery query)
            where TContract : class
            where TOut : class;

        /// <summary>
        /// Lets you invoke stored procedures that returns Json.
        /// This is the most effective return type, since the Json is stored in the database,
        /// no deserialization will take place. 
        /// </summary>
        /// <typeparam name="T">
        /// Structure type, used as a contract defining the scheme.</typeparam>
        /// <param name="query"></param>
        /// <returns>Json representation of structures (<typeparamref name="T"/>)
        /// or empty IEnumerable of <see cref="string"/>.</returns>
        IEnumerable<string> NamedQueryAsJson<T>(INamedQuery query)
            where T : class;
    }
}