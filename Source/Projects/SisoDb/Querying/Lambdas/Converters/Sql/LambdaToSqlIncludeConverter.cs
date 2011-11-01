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
            var sqlIncludes = new List<SqlInclude>();
            const string jsonColumnDefinitionFormat = "min(cs{0}.{1}) as [{2}]";
            const string joinFormat = "left join [dbo].[{0}] as cs{1} on cs{1}.[{2}] = si.[{3}] and si.[{4}]='{5}'";

            foreach (var includeNode in lambda.Nodes.Cast<IncludeNode>())
            {
                var includeCount = sqlIncludes.Count;
                var parentMemberPath = includeNode.IdReferencePath;

                var jsonColumnDefinition = jsonColumnDefinitionFormat.Inject(
                    includeCount,
                    StructureStorageSchema.Fields.Json.Name,
                    includeNode.ObjectReferencePath);

                var join = joinFormat.Inject(
                    includeNode.ChildStructureName + "Structure",
                    includeCount,
                    StructureStorageSchema.Fields.Id.Name,
                    IndexStorageSchema.GetValueSchemaFieldForType(includeNode.MemberType).Name,
                    IndexStorageSchema.Fields.MemberPath.Name,
                    parentMemberPath);
                
                sqlIncludes.Add(new SqlInclude(jsonColumnDefinition, join));
            }

            return sqlIncludes;
        }
    }
}