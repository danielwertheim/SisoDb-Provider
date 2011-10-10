using System.Collections.Generic;
using System.Linq;
using EnsureThat;
using NCore;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;

namespace SisoDb.Querying.Lambdas.Processors.Sql
{
    public class ParsedIncludeSqlProcessor : IParsedLambdaProcessor<IList<ISqlInclude>>
    {
        public IMemberPathGenerator MemberPathGenerator { private get; set; }

        public ParsedIncludeSqlProcessor(IMemberPathGenerator memberPathGenerator)
        {
            Ensure.That(() => memberPathGenerator).IsNotNull();
            MemberPathGenerator = memberPathGenerator;
        }

        public IList<ISqlInclude> Process(IParsedLambda lambda)
        {
            var sqlIncludes = new List<ISqlInclude>();
            const string sqlFormat = "(select cs{0}.[json] from [dbo].[{1}] as cs{0} where si.[{2}] = cs{0}.StructureId) as [{3}]";

            foreach (var includeNode in lambda.Nodes.Cast<IncludeNode>())
            {
                var includeCount = sqlIncludes.Count;
                var parentMemberPath = MemberPathGenerator.Generate(includeNode.IdReferencePath);
                var sql = sqlFormat.Inject(
                    includeCount,
                    includeNode.ChildStructureName + "Structure",
                    parentMemberPath,
                    includeNode.ObjectReferencePath);
                
                sqlIncludes.Add(new SqlInclude(sql));
            }

            return sqlIncludes;
        }
    }
}