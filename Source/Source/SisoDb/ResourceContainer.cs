using System;
using SisoDb.Cryptography;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;

namespace SisoDb
{
    public class ResourceContainer
    {
        private readonly IHashService _defaultHashService;
        private readonly ISchemaBuilder _defaultSchemaBuilder;
        private readonly IJsonSerializer _defaultJsonSerializer;
        private readonly IStructureTypeReflecter _defaultStructureTypeReflecter;
        private readonly IMemberNameGenerator _defaultMemberNameGenerator;
        private readonly IStructureTypeFactory _defaultStructureTypeFactory;
        private readonly IStructureSchemas _defaultStructureSchemas;

        public Func<IHashService> ResolveHashService;
        public Func<IJsonSerializer> ResolveJsonSerializer;
        public Func<IMemberNameGenerator> ResolveMemberNameGenerator;
        public Func<ISchemaBuilder> ResolveSchemaBuilder;
        public Func<IStructureTypeReflecter> ResolveStructureTypeReflecter;
        public Func<IStructureTypeFactory> ResolveStructureTypeFactory;
        public Func<IStructureSchemas> ResolveStructureSchemas;

        public ResourceContainer()
        {
            _defaultHashService = new HashService();
            _defaultJsonSerializer = new ServiceStackJsonSerializer();
            _defaultMemberNameGenerator = new HashMemberNameGenerator(_defaultHashService);
            _defaultSchemaBuilder = new AutoSchemaBuilder(_defaultHashService);
            _defaultStructureTypeReflecter = new StructureTypeReflecter();
            _defaultStructureTypeFactory = new StructureTypeFactory(_defaultStructureTypeReflecter);
            _defaultStructureSchemas = new StructureSchemas(_defaultStructureTypeFactory, _defaultSchemaBuilder);

            ResolveHashService = () => _defaultHashService;
            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveMemberNameGenerator = () => _defaultMemberNameGenerator;
            ResolveSchemaBuilder = () => _defaultSchemaBuilder;
            ResolveStructureTypeReflecter = () => _defaultStructureTypeReflecter;
            ResolveStructureTypeFactory = () => _defaultStructureTypeFactory;
            ResolveStructureSchemas = () => _defaultStructureSchemas;
        }
    }
}