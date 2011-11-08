using System.Collections.Generic;
using System.Linq;
using NCore;
using PineCone.Structures.Schemas;
using SisoDb.DbSchema;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;

namespace SisoDb.Querying.Lambdas.Converters.Sql
{
    public class LambdaToSqlIncludeConverter : ILambdaToSqlIncludeConverter
    {
        public IList<SqlInclude> Convert(IStructureSchema structureSchema, IParsedLambda lambda)
        {
            var includes = new List<SqlInclude>();

            if (lambda == null || lambda.Nodes.Count == 0)
                return includes;

            const string joinFormat = "left join [{0}] as cs{1} on cs{1}.[{2}] = si.[{3}] and si.[{4}]='{5}'";

            foreach (var includeNode in lambda.Nodes.Cast<IncludeNode>())
            {
                var includeCount = includes.Count;
                var parentMemberPath = includeNode.IdReferencePath;

                var jsonColumnDefinition = "min(cs{0}.Json) as [{1}Json]".Inject(
                    includeCount,
                    includeNode.ObjectReferencePath);

                var join = joinFormat.Inject(
                    includeNode.ChildStructureName + "Structure",
                    includeCount,
                    StructureStorageSchema.Fields.Id.Name,
                    IndexStorageSchema.GetValueSchemaFieldForType(includeNode.MemberType).Name,
                    IndexStorageSchema.Fields.MemberPath.Name,
                    parentMemberPath);
                
                includes.Add(new SqlInclude(jsonColumnDefinition, join));
            }

            return includes;
        }
    }
}