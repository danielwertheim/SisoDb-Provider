using System.Collections.Generic;
using System.Linq;
using SisoDb.Lambdas.Nodes;
using SisoDb.Querying;

namespace SisoDb.Lambdas.Processors
{
    public class ParsedIncludeSqlProcessor : IParsedLambdaProcessor<IList<ISqlInclude>>
    {
        public IMemberNameGenerator MemberNameGenerator { private get; set; }

        public ParsedIncludeSqlProcessor(IMemberNameGenerator memberNameGenerator)
        {
            MemberNameGenerator = memberNameGenerator.AssertNotNull("memberNameGenerator");
        }

        public IList<ISqlInclude> Process(IParsedLambda lambda)
        {
            var sqlIncludes = new List<ISqlInclude>();
            const string sqlFormat = "(select cs{0}.[json] from [dbo].[{1}] as cs{0} where si.[{2}] = cs{0}.Id) as [{3}]";

            foreach (var includeNode in lambda.Nodes.Cast<IncludeNode>())
            {
                var includeCount = sqlIncludes.Count;
                var parentMemberPath = MemberNameGenerator.Generate(includeNode.IdReferencePath);
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