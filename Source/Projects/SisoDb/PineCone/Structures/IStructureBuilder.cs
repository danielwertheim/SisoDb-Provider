using PineCone.Serializers;
using PineCone.Structures.Schemas;

namespace PineCone.Structures
{
    /// <summary>
    /// Builds <see cref="IStructure"/> instances from sent Items.
    /// </summary>
    public interface IStructureBuilder
    {
        /// <summary>
        /// Creates the indexes for the <see cref="IStructure"/>.
        /// </summary>
        IStructureIndexesFactory IndexesFactory { get; set; }

        /// <summary>
        /// Serilizer used to populate <see cref="IStructure.Data"/>.
        /// </summary>
        IStructureSerializer StructureSerializer { get; set; }

        /// <summary>
        /// Responsible for generating <see cref="IStructureId"/>.
        /// </summary>
        IStructureIdGenerator StructureIdGenerator { get; set; }

        /// <summary>
        /// Creates a single <see cref="IStructure"/> for sent <typeparamref name="T"/> item.
        /// The item will be assigned a new Sequential Guid Id as StructureId.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="structureSchema"></param>
        /// <returns></returns>
        IStructure CreateStructure<T>(T item, IStructureSchema structureSchema) where T : class;

        /// <summary>
        /// Yields each item as an <see cref="IStructure"/>.
        /// All items will be assigned a new Sequential Guid Id as StructureId.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="structureSchema"></param>
        /// <returns></returns>
        IStructure[] CreateStructures<T>(T[] items, IStructureSchema structureSchema) where T : class;
    }
}