using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Lambdas
{
    internal static class MemberExpressionExtensions
    {
        internal static string Path(this MemberExpression e)
        {
            var path = "";
            var parent = e.Expression as MemberExpression;

            if (parent != null)
                path = parent.Path() + ".";

            return path + e.Member.Name;
        }

        internal static IList<MemberExpression> ExtractGraphLineFirstToLast(this MemberExpression e)
        {
            var nodes = new List<MemberExpression>();
            
            var parent = e.Expression as MemberExpression;
            if (parent != null)
                nodes.AddRange(ExtractGraphLineFirstToLast(parent));

            nodes.Add(e);

            return nodes;
        }
    }
}