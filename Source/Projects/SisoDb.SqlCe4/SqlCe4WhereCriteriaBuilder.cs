using System.Text;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.Querying.Sql;

namespace SisoDb.SqlCe4
{
    public class SqlCe4WhereCriteriaBuilder : SqlWhereCriteriaBuilder
    {
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