using System.Linq;
using PineCone.Structures.Schemas;
using PineCone.Structures.Schemas.MemberAccessors;

namespace SisoDb.Diagnostics.Appenders
{
    public class StructureSchemasAppender : IDiagnosticsContextAppender<IStructureSchemas>
    {
        protected readonly DiagnosticsContext Context;

        public StructureSchemasAppender(DiagnosticsContext context)
        {
            Context = context;
        }

        public virtual void Append(IStructureSchemas structureSchemas)
        {
            var section = Context.AddSection("StructureSchemas");
            foreach (var schema in structureSchemas.GetSchemas().OrderBy(s => s.Name))
            {
                var schemaGrp = new DiagnosticsGroup(schema.Name);
                section.AddGroup(schemaGrp);
                OnAppendStructureSchema(schemaGrp, schema);

                for (var i = 0; i < schema.IndexAccessors.Count; i++)
                {
                    var indexAccessor = schema.IndexAccessors[i];
                    var iacGrp = new DiagnosticsGroup("{0}.IndexAccessor[{1}]", schema.Name, i);
                    section.AddGroup(iacGrp);
                    OnAppendIndexAccessor(iacGrp, indexAccessor);
                }
            }
        }

        protected virtual void OnAppendStructureSchema(DiagnosticsGroup group, IStructureSchema schema)
        {
            group
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
        }

        protected virtual void OnAppendIndexAccessor(DiagnosticsGroup group, IIndexAccessor indexAccessor)
        {
            group
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