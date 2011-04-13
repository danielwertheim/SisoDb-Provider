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
        private readonly IStructureSchemas _defaultStructureSchemas;
        private readonly IJsonSerializer _defaultJsonSerializer;
        private readonly IStructureTypeReflecter _defaultStructureTypeReflecter;
        private readonly IMemberNameGenerator _defaultMemberNameGenerator;

        public Func<IHashService> ResolveHashService;
        public Func<IJsonSerializer> ResolveJsonSerializer;
        public Func<IMemberNameGenerator> ResolveMemberNameGenerator;
        public Func<ISchemaBuilder> ResolveSchemaBuilder;
        public Func<IStructureSchemas> ResolveStructureSchemas;
        public Func<IStructureTypeReflecter> ResolveStructureTypeReflecter;
        
        public ResourceContainer()
        {
            _defaultHashService = new HashService();
            _defaultJsonSerializer = new ServiceStackJsonSerializer();
            _defaultMemberNameGenerator = new HashMemberNameGenerator(_defaultHashService);
            _defaultSchemaBuilder = new AutoSchemaBuilder(_defaultHashService);
            _defaultStructureSchemas = new StructureSchemas(_defaultSchemaBuilder);
            _defaultStructureTypeReflecter = new StructureTypeReflecter();

            ResolveHashService = () => _defaultHashService;
            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveMemberNameGenerator = () => _defaultMemberNameGenerator;
            ResolveSchemaBuilder = () => _defaultSchemaBuilder;
            ResolveStructureSchemas = () => _defaultStructureSchemas;
            ResolveStructureTypeReflecter = () => _defaultStructureTypeReflecter;
        }
    }
}