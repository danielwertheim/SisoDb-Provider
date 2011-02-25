using System.Collections.Generic;
using System.Linq.Expressions;

namespace SisoDb.Lambdas
{
    public static class ExpressionExtensions
    {
        public static string Path(this MemberExpression e)
        {
            var path = "";
            var parent = e.Expression as MemberExpression;

            if (parent != null)
                path = parent.Path() + ".";

            return path + e.Member.Name;
        }

        public static IList<MemberExpression> ExtractGraphLineFirstToLast(this MemberExpression e)
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