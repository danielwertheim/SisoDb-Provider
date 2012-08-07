using System;
using System.Collections.Generic;

namespace SisoDb.Serialization
{
    public interface ISisoDbSerializer
    {
        /// <summary>
        /// Configuration options.
        /// </summary>
        SerializerOptions Options { get; set; }

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
        /// Deserializes sent JSON as <typeparamref name="TTemplate"/> and
        /// enables support for Anonymous types, hence <typeparamref name="TTemplate"/>
        /// could be an anonymous type.
        /// </summary>
        /// <typeparam name="TTemplate"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        TTemplate DeserializeUsingTemplate<TTemplate>(string json) where TTemplate : class;

        /// <summary>
        /// Deserializes sent JSON to yielded stream of <typeparamref name="T"/>.
        /// Either done sequentially or in parallel. It is controlled by <see cref="Options"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceData"></param>
        /// <returns></returns>
        IEnumerable<T> DeserializeMany<T>(IEnumerable<string> sourceData) where T : class;

        /// <summary>
        /// Deserializes sent JSON to yielded stream of <paramref name="structureType"/>.
        /// Either done sequentially or in parallel. It is controlled by <see cref="Options"/>
        /// </summary>
        /// <param name="sourceData"></param>
        /// <param name="structureType"></param>
        /// <returns></returns>
        IEnumerable<object> DeserializeMany(IEnumerable<string> sourceData, Type structureType);

        /// <summary>
        /// Deserializes sent JSON to yielded stream of <typeparamref name="TTemplate"/> using <paramref name="templateType"/>.
        /// Enables support for Anonymous types, hence <paramref name="templateType"/> could be an anonymous type.
        /// Either done sequentially or in parallel. It is controlled by <see cref="Options"/>
        /// </summary>
        /// <typeparam name="TTemplate"></typeparam>
        /// <param name="sourceData"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        IEnumerable<TTemplate> DeserializeManyUsingTemplate<TTemplate>(IEnumerable<string> sourceData, Type templateType) where TTemplate : class;
    }
}