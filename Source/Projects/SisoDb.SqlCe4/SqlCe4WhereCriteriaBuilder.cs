using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4WhereCriteriaBuilder : SqlServerWhereCriteriaBuilder
    {
        protected override string GetStringMaxDataLength(StringEqualsMemberNode member)
        {
            return member.IsTextType ? "4000" : "300";
        }
    }
}
