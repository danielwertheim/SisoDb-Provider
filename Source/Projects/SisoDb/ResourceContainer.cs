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
        private readonly IMemberPathGenerator _defaultMemberPathGenerator;
        private readonly IStructureSchemas _defaultStructureSchemas;
        private readonly IStructureBuilder _defaultStructureBuilder;

        public readonly Func<IJsonSerializer> ResolveJsonSerializer;
        public readonly Func<IMemberPathGenerator> ResolveMemberPathGenerator;
        public readonly Func<IStructureSchemas> ResolveStructureSchemas;
        public readonly Func<IStructureBuilder> ResolveStructureBuilder;

        public ResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();
            _defaultMemberPathGenerator = new SimpleMemberPathGenerator();
            _defaultStructureSchemas = new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            _defaultStructureBuilder = new StructureBuilder();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveMemberPathGenerator = () => _defaultMemberPathGenerator;
            ResolveStructureSchemas = () => _defaultStructureSchemas;
            ResolveStructureBuilder = () => _defaultStructureBuilder;
        }
    }
}