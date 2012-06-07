using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlServer
{
    public class SqlServerWhereCriteriaBuilder : SqlWhereCriteriaBuilder
    {
        protected override string GetMemberFormatString(MemberNode member)
        {
            var stringEquals = member as StringEqualsMemberNode;

            if (stringEquals != null && stringEquals.ExactMatch)
            {
                var format = "(convert(varbinary({0}), {{0}}) = convert(varbinary({0}), {{2}}) AND {{0}}{{1}}{{2}})";
                return string.Format(format, GetStringMaxDataLength(stringEquals));
            }

            return base.GetMemberFormatString(member);
        }

        protected virtual string GetStringMaxDataLength(StringEqualsMemberNode member)
        {
            return member.IsTextType ? "max" : "300";
        }
    }
}
