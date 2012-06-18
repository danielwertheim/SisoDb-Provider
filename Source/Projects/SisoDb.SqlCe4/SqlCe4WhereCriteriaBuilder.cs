using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;
using SisoDb.SqlServer;

namespace SisoDb.SqlCe4
{
    public class SqlCe4WhereCriteriaBuilder : SqlServerWhereCriteriaBuilder
    {
        protected override string GetStringMaxDataLength(StringEqualsMemberNode member)
        {
            return member.IsTextType ? "4000" : "300";
        }

        public override void AddSetOfValues(ArrayValueNode valueNode)
        {
            var tmp = new StringBuilder();
            foreach (var value in valueNode.Value)
            {
                if(tmp.Length > 0) tmp.Append(",");

                var param = new DacParameter(GetNextParameterName(), value);
                Params.Add(param);

                tmp.Append(param.Name);
            }

            AddValue(string.Format("({0})", tmp));
        }
    }
}
