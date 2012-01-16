using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using SisoDb.Querying.Lambdas.Nodes;
using NCore.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class IncludeParser : IIncludeParser
    {
        public IParsedLambda Parse(string includedStructureSetName, LambdaExpression[] includeExpressions)
        {
			Ensure.That(includedStructureSetName, "includedStructureSetName").IsNotNullOrWhiteSpace();
            Ensure.That(includeExpressions, "includeExpressions").HasItems();

            var nodes = new NodesCollection();

            foreach (var includeExpression in includeExpressions)
            {
                var memberExpression = includeExpression.GetRightMostMember();
                var idReferencePath = memberExpression.ToPath();
                var objectReferencePath = BuildObjectReferencePath(idReferencePath);

                nodes.AddNode(
                    new IncludeNode(includedStructureSetName, idReferencePath, objectReferencePath, memberExpression.Type));    
            }

            return new ParsedLambda(nodes.ToArray());
        }

        private static string BuildObjectReferencePath(string idReferencePath)
        {
            return !idReferencePath.EndsWith("Id") 
                ? idReferencePath 
                : idReferencePath.Substring(0, idReferencePath.Length - 2);
        }
    }
}