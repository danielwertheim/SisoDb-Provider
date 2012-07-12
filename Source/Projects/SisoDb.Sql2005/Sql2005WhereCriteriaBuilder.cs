using System.Text;
using SisoDb.Dac;
using SisoDb.Querying.Lambdas.Nodes;
using SisoDb.SqlServer;

namespace SisoDb.Sql2005
{
    public class Sql2005WhereCriteriaBuilder : SqlServerWhereCriteriaBuilder
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