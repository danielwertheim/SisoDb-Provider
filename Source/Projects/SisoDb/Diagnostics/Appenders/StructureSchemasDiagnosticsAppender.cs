using System.Linq;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.Configuration;
using PineCone.Structures.Schemas.MemberAccessors;

namespace SisoDb.Diagnostics.Appenders
{
    public class StructureSchemasDiagnosticsAppender : IDiagnosticsAppender<IStructureSchemas>
    {
        protected readonly DiagnosticsInfo Info;

        public StructureSchemasDiagnosticsAppender(DiagnosticsInfo info)
        {
            Info = info;
        }

        public virtual void Append(IStructureSchemas structureSchemas)
        {
            var group = Info.AddGroup("StructureSchemas");
            foreach (var schema in structureSchemas.GetSchemas().OrderBy(s => s.Name))
            {
                var typeConfig = structureSchemas.StructureTypeFactory.Configurations.GetConfiguration(schema.Type.Type);
                OnAppendStructureSchema(group, schema, typeConfig);
            }
        }

        protected virtual void OnAppendStructureSchema(DiagnosticsGroup parent, IStructureSchema schema, IStructureTypeConfig typeConfig)
        {
            var group = parent.AddGroup(schema.Name)
                .AddNode("Name", schema.Name)
                .AddNode("Hash", schema.Hash)
                .AddNode("HasId", schema.HasId)
                .AddNode("HasConcurrencyToken", schema.HasConcurrencyToken)
                .AddNode("HasTimeStamp", schema.HasTimeStamp);

            if (schema.HasId)
            {
                group
                    .AddNode("IdAccessor.Path", schema.IdAccessor.Path)
                    .AddNode("IdAccessor.DataType", schema.IdAccessor.DataType)
                    .AddNode("IdAccessor.IdType", schema.IdAccessor.IdType);
            }

            if (schema.HasConcurrencyToken)
            {
                group
                    .AddNode("ConcurrencyTokenAccessor.Path", schema.ConcurrencyTokenAccessor.Path)
                    .AddNode("ConcurrencyTokenAccessor.DataType", schema.ConcurrencyTokenAccessor.DataType);
            }

            if (schema.HasTimeStamp)
            {
                group
                    .AddNode("TimeStampAccessor.Path", schema.TimeStampAccessor.Path)
                    .AddNode("TimeStampAccessor.DataType", schema.TimeStampAccessor.DataType);
            }

            if(typeConfig != null)
                OnAppendNonIndexedMemberPaths(group, typeConfig);

            if(schema.IndexAccessors.Any())
                OnAppendIndexAccessors(group, schema);
        }

        protected virtual void OnAppendNonIndexedMemberPaths(DiagnosticsGroup parent, IStructureTypeConfig typeConfig)
        {
            var groupOfNotIndexedPaths = parent.AddGroup("Member paths NOT being indexed");
            foreach (var notIndexed in typeConfig.MemberPathsNotBeingIndexed)
                groupOfNotIndexedPaths.AddNode(notIndexed, null);
        }

        protected virtual void OnAppendIndexAccessors(DiagnosticsGroup parent, IStructureSchema schema)
        {
            var indexAccessorsGroup = parent.AddGroup("Indexes");
            foreach (var indexAccessor in schema.IndexAccessors)
                OnAppendIndexAccessor(indexAccessorsGroup, schema, indexAccessor);
        }

        protected virtual void OnAppendIndexAccessor(DiagnosticsGroup parent, IStructureSchema schema, IIndexAccessor indexAccessor)
        {
            var group = parent.AddGroup(indexAccessor.Path)
                .AddNode("Path", indexAccessor.Path)
                .AddNode("DataType", indexAccessor.DataType)

                .AddNode("IsElement", indexAccessor.IsElement)
                .AddNode("IsEnumerable", indexAccessor.IsEnumerable)
                .AddNode("IsUnique", indexAccessor.IsUnique);

            if (indexAccessor.IsUnique && indexAccessor.UniqueMode.HasValue)
                group.AddNode("UniqueMode", indexAccessor.UniqueMode.Value);
        }
    }
}