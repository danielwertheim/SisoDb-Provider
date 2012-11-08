using System;
using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public interface ISisoSerializer
    {
        /// <summary>
        /// Serializes sent <typeparamref name="T"/> to JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        string Serialize<T>(T item) where T : class;

        /// <summary>
        /// Serializes many <typeparamref name="T"/> to stream of JSON.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        IEnumerable<string> SerializeMany<T>(IEnumerable<T> items) where T : class;
        
        /// <summary>
        /// Deserializes sent JSON as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        T Deserialize<T>(string json) where T : class;

        /// <summary>
        /// Deserializes sent JSON as <paramref name="structureType"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="structureType"></param>
        /// <returns></returns>
        object Deserialize(string json, Type structureType);

        /// <summary>
        /// Deserializes sent JSON to yielded stream of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class;

        /// <summary>
        /// Deserializes sent JSON to yielded stream of <paramref name="structureType"/>.
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="structureType"></param>
        /// <returns></returns>
        IEnumerable<object> DeserializeMany(IEnumerable<string> sourceData, Type structureType);
    }
}