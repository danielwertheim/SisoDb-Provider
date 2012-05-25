using System.Text;

namespace SisoDb.Querying.Lambdas.Nodes
{
    public static class NodesExtensions
    {
         public static string AsString(this INode[] nodes)
         {
             var sb = new StringBuilder();
             foreach (var node in nodes)
             {
                 sb.Append(node);

                 if(node is OperatorNode)
                    sb.Append(" ");
             }
             return sb.ToString();
         }

         public static StartsWithMemberNode ToStartsWithNode(this MemberNode memberNode)
         {
             return new StartsWithMemberNode(memberNode.Path, memberNode.DataType, memberNode.DataTypeCode);
         }

         public static EndsWithMemberNode ToEndsWithNode(this MemberNode memberNode)
         {
             return new EndsWithMemberNode(memberNode.Path, memberNode.DataType, memberNode.DataTypeCode);
         }

         public static ToLowerMemberNode ToLowerNode(this MemberNode memberNode)
         {
             return new ToLowerMemberNode(memberNode.Path, memberNode.DataType, memberNode.DataTypeCode);
         }

         public static ToUpperMemberNode ToUpperNode(this MemberNode memberNode)
         {
             return new ToUpperMemberNode(memberNode.Path, memberNode.DataType, memberNode.DataTypeCode);
         }
    }
}