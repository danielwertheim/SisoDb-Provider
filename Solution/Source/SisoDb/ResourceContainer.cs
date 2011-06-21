using System;
using SisoDb.Cryptography;
using SisoDb.DbSchema;
using SisoDb.Serialization;
using SisoDb.Structures;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;

namespace SisoDb
{
    public class ResourceContainer
    {
        private readonly IHashService _defaultHashService;
        private readonly ISchemaBuilder _defaultSchemaBuilder;
        private readonly IJsonSerializer _defaultJsonSerializer;
        private readonly IStructureTypeReflecter _defaultStructureTypeReflecter;
        private readonly IMemberNameGenerator _defaultMemberNameGenerator;

        //SHARED AS DEFAULT
        public Func<IHashService> ResolveHashService;
        public Func<IJsonSerializer> ResolveJsonSerializer;
        public Func<IMemberNameGenerator> ResolveMemberNameGenerator;
        public Func<ISchemaBuilder> ResolveSchemaBuilder;
        public Func<IStructureTypeReflecter> ResolveStructureTypeReflecter;

        //NON SHARED AS DEFAULT
        public Func<IStructureTypeFactory> ResolveStructureTypeFactory;
        public Func<IStructureSchemas> ResolveStructureSchemas;
        public Func<ISisoIdFactory> ResolveSisoIdFactory;
        public Func<IStructureIndexesFactory> ResolveStructureIndexesFactory;
        public Func<IStructureBuilder> ResolveStructureBuilder;
        public Func<IDbSchemaManager> ResolveDbSchemaManager;
        public Func<IJsonBatchDeserializer> ResolveJsonBatchDeserializer;

        public ResourceContainer()
        {
            _defaultHashService = new HashService();
            _defaultJsonSerializer = new ServiceStackJsonSerializer();
            _defaultMemberNameGenerator = new HashMemberNameGenerator(_defaultHashService);
            _defaultSchemaBuilder = new AutoSchemaBuilder(_defaultHashService);
            _defaultStructureTypeReflecter = new StructureTypeReflecter();

            ResolveHashService = () => _defaultHashService;
            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveMemberNameGenerator = () => _defaultMemberNameGenerator;
            ResolveSchemaBuilder = () => _defaultSchemaBuilder;
            ResolveStructureTypeReflecter = () => _defaultStructureTypeReflecter;

            ResolveStructureTypeFactory = () => new StructureTypeFactory(_defaultStructureTypeReflecter);
            ResolveStructureSchemas = () => new StructureSchemas(ResolveStructureTypeFactory(), _defaultSchemaBuilder);
            ResolveSisoIdFactory = () => new SisoIdFactory();
            ResolveStructureIndexesFactory = () => new StructureIndexesFactory(SisoEnvironment.Formatting.StringConverter);
            ResolveStructureBuilder = () => new StructureBuilder(ResolveJsonSerializer(), ResolveSisoIdFactory(), ResolveStructureIndexesFactory());
            ResolveDbSchemaManager = () => new DbSchemaManager();
            ResolveJsonBatchDeserializer = () => new ParallelJsonBatchDeserializer(ResolveJsonSerializer());
        }
    }
}