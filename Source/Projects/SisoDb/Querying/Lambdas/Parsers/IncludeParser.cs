using System.Linq;
using System.Linq.Expressions;
using EnsureThat;
using PineCone.Structures.Schemas;
using SisoDb.Querying.Lambdas.Nodes;
using NCore.Expressions;

namespace SisoDb.Querying.Lambdas.Parsers
{
    public class IncludeParser : IIncludeParser
    {
        protected readonly IDataTypeConverter DataTypeConverter;

        public IncludeParser(IDataTypeConverter dataTypeConverter)
        {
            Ensure.That(dataTypeConverter, "dataTypeConverter").IsNotNull();
            DataTypeConverter = dataTypeConverter;
        }

        public virtual IParsedLambda Parse(string includedStructureSetName, LambdaExpression[] includeExpressions)
        {
			Ensure.That(includedStructureSetName, "includedStructureSetName").IsNotNullOrWhiteSpace();
            Ensure.That(includeExpressions, "includeExpressions").HasItems();

            var nodes = new NodesCollection();

            foreach (var includeExpression in includeExpressions)
            {
                var memberExpression = includeExpression.GetRightMostMember();
                var idReferencePath = memberExpression.ToPath();
                var objectReferencePath = BuildObjectReferencePath(idReferencePath);

                nodes.AddNode(new IncludeNode(
                    includedStructureSetName, idReferencePath, objectReferencePath, memberExpression.Type, DataTypeConverter.Convert(memberExpression.Type, idReferencePath)));    
            }

            return new ParsedLambda(nodes.ToArray());
        }

        protected virtual string BuildObjectReferencePath(string idReferencePath)
        {
            return !idReferencePath.EndsWith("Id") 
                ? idReferencePath 
                : idReferencePath.Substring(0, idReferencePath.Length - 2);
        }
    }
}