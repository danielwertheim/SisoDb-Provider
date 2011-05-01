using System.Collections.Generic;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    /// <summary>
    /// Builds <see cref="IStructure"/> instances from sent
    /// Items.
    /// </summary>
    public interface IStructureBuilder
    {
        /// <summary>
        /// Serializer used for generating the JSON-representation of sent Items.
        /// </summary>
        IJsonSerializer JsonSerializer { get; }

        /// <summary>
        /// Factory for creating <see cref="ISisoId"/> from the Items.
        /// </summary>
        ISisoIdFactory SisoIdFactory { get; }
        
        /// <summary>
        /// Factory for creating <see cref="IStructureIndex"/> for each
        /// indexable member of the Items.
        /// </summary>
        IStructureIndexesFactory IndexesFactory { get; }

        /// <summary>
        /// Creates a single <see cref="IStructure"/> for sent <typeparamref name="T"/> item.
        /// The <typeparamref name="T"/> item needs to have its Id assigned, if not an
        /// exception will be thrown.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="structureSchema"></param>
        /// <returns></returns>
        IStructure CreateStructure<T>(T item, IStructureSchema structureSchema) where T : class;

        /// <summary>
        /// Creates batches of <see cref="IStructure"/> items with Guid Ids.
        /// All items will be assigned a new Sequential Guid Id.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="structureSchema"></param>
        /// <param name="maxBatchSize"></param>
        /// <returns></returns>
        IEnumerable<IStructure[]> CreateBatchedGuidStructures<T>(IEnumerable<T> items, IStructureSchema structureSchema, int maxBatchSize) 
            where T : class;

        /// <summary>
        /// Creates batches of <see cref="IStructure"/> items with Identity Ids.
        /// All items will be assigned a new identity Id. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="structureSchema"></param>
        /// <param name="maxBatchSize"></param>
        /// <param name="identitySeed"></param>
        /// <returns></returns>
        IEnumerable<IStructure[]> CreateBatchedIdentityStructures<T>(IEnumerable<T> items, IStructureSchema structureSchema, int maxBatchSize, int identitySeed)
            where T : class;
    }
}