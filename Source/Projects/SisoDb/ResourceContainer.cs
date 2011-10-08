using System;
using PineCone;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Builders;
using PineCone.Structures.Schemas.Configuration;
using SisoDb.DbSchema;
using SisoDb.Serialization;

namespace SisoDb
{
    public class ResourceContainer
    {
        private readonly IJsonSerializer _defaultJsonSerializer;
        private readonly IMemberNameGenerator _defaultMemberNameGenerator;
        private readonly IStructureTypeReflecter _defaultStructureTypeReflecter;

        public readonly Func<IJsonSerializer> ResolveJsonSerializer;
        public readonly Func<IMemberNameGenerator> ResolveMemberNameGenerator;
        public readonly Func<IStructureTypeReflecter> ResolveStructureTypeReflecter;

        public Func<IDbSchemaManager> ResolveDbSchemaManager;
        public Func<IStructureSchemas> ResolveStructureSchemas;
        public Func<IStructureBuilder> ResolveStructureBuilder;

        public ResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();
            _defaultMemberNameGenerator = new MemberNameGenerator();
            _defaultStructureTypeReflecter = new StructureTypeReflecter();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveMemberNameGenerator = () => _defaultMemberNameGenerator;
            ResolveStructureTypeReflecter = () => _defaultStructureTypeReflecter;

            ResolveDbSchemaManager = () => new DbSchemaManager();
            ResolveStructureBuilder = () => new StructureBuilder(new StructureIdGenerators(), new StructureIndexesFactory());

            var structureTypeFactory = new StructureTypeFactory(ResolveStructureTypeReflecter(), new StructureTypeConfigurations());
            var schemaBuilder = new AutoSchemaBuilder();

            ResolveStructureSchemas = () => new StructureSchemas(structureTypeFactory, schemaBuilder);
        }
    }
}