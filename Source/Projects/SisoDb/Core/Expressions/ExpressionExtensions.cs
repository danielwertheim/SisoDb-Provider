using System.Linq.Expressions;

namespace SisoDb.Core.Expressions
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
    }
}