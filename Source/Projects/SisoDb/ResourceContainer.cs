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
        
        public readonly Func<IJsonSerializer> ResolveJsonSerializer;
        public readonly Func<IMemberPathGenerator> ResolveMemberPathGenerator;
        public readonly Func<IStructureSchemas> ResolveStructureSchemas;
        public readonly Func<IStructureBuilder> ResolveStructureBuilder;

        public ResourceContainer()
        {
            _defaultJsonSerializer = new ServiceStackJsonSerializer();
            _defaultMemberPathGenerator = new SimpleMemberPathGenerator();

            ResolveJsonSerializer = () => _defaultJsonSerializer;
            ResolveMemberPathGenerator = () => _defaultMemberPathGenerator;
            ResolveStructureSchemas = () => new StructureSchemas(new StructureTypeFactory(), new AutoSchemaBuilder());
            ResolveStructureBuilder = () => new StructureBuilder();
        }
    }
}