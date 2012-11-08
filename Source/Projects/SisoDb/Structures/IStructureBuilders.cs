using System;
using SisoDb.Dac;
using SisoDb.Serialization;
using SisoDb.Structures.IdGenerators;
using SisoDb.Structures.Schemas;

namespace SisoDb.Structures
{
    public interface IStructureBuilders 
    {
        /// <summary>
        /// Resolves the <see cref="IStructureIdGenerator"/> responsible
        /// for generating GUIDs. Defaults to <see cref="SequentialGuidStructureIdGenerator"/>.
        /// </summary>
        Func<IStructureSchema, IStructureIdGenerator> GuidStructureIdGeneratorFn { get; set; }
        /// <summary>
        /// Resolves the <see cref="IIdentityStructureIdGenerator"/> responsible
        /// for generating identities. Defaults to <see cref="DbIdentityStructureIdGenerator"/>.
        /// </summary>
        Func<IStructureSchema, IDbClient, IIdentityStructureIdGenerator> IdentityStructureIdGeneratorFn { get; set; }
        /// <summary>
        /// Resolves the serializer to use. Maps to <see cref="ISisoDatabase.Serializer"/>.
        /// </summary>
        Func<ISisoSerializer> SerializerFn { get; }
        /// <summary>
        /// Resolves the <see cref="IStructureSerializer"/> responsible for serializing a structure.
        /// Defaults to <see cref="StructureSerializer"/>. Will use <see cref="SerializerFn"/> to get
        /// the actual serializer, hence normally nothing you would like to switch.
        /// </summary>
        Func<IStructureSerializer> StructureSerializerFn { get; set; }
        /// <summary>
        /// Resolves the <see cref="IStructureBuilder"/> to use for the passed <see cref="IStructureSchema"/>
        /// when inserting new structures.
        /// </summary>
        Func<IStructureSchema, IDbClient, IStructureBuilder> ResolveBuilderForInsertsBy { set; }
        /// Resolves the <see cref="IStructureBuilder"/> to use for the passed <see cref="IStructureSchema"/>
        /// when updating existing structures.
        Func<IStructureSchema, IStructureBuilder> ResolveBuilderForUpdatesBy { set; }
        /// <summary>
        /// Uses <see cref="ResolveBuilderForInsertsBy"/> to create an <see cref="IStructureBuilder"/>
        /// used when inserting new structures.
        /// </summary>
        /// <param name="structureSchema"></param>
        /// <param name="dbClient"></param>
        /// <returns></returns>
        IStructureBuilder ResolveBuilderForInsert(IStructureSchema structureSchema, IDbClient dbClient);
        /// <summary>
        /// Uses <see cref="ResolveBuilderForUpdatesBy"/> to create an <see cref="IStructureBuilder"/>
        /// used when updating existing structures.
        /// </summary>
        /// <param name="structureSchema"></param>
        /// <returns></returns>
        IStructureBuilder ResolveBuilderForUpdate(IStructureSchema structureSchema);
    }
}