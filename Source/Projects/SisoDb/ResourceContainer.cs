using System;
using PineCone.Structures;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Builders;
using SisoDb.Serialization;

namespace SisoDb
{
    public class ResourceContainer
    {
        private readonly IJsonSerializer _defaultJsonSerializer;
        private readonly IMemberNameGenerator _defaultMemberNameGenerator;
        private readonly IStructureSchemas _defaultStructureSchemas;
        private readonly IStructureBuilder _defaultStructureBuilder;

        public readonly Func<IJsonSerializer> ResolveJsonSerializer;
        public readonly Func<IMemberNameGenerator> ResolveMemberNameGenerator;
        public readonly Func<IStructureSchemas> ResolveStructureSchemas;
        public readonly Func<IStructureBuilder> ResolveStructureBuilder;

        public ResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();
            _defaultMemberNameGenerator = new MemberNameGenerator();
            _defaultStructureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            _defaultStructureBuilder = new StructureBuilder();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveMemberNameGenerator = () => _defaultMemberNameGenerator;
            ResolveStructureSchemas = () => _defaultStructureSchemas;
            ResolveStructureBuilder = () => _defaultStructureBuilder;
        }
    }
}