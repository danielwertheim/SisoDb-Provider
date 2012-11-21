using System.Text;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.NCore.Cryptography;
using SisoDb.Resources;

namespace SisoDb.Querying.Sql
{
    public class DefaultDbQueryChecksumGenerator : IDbQueryChecksumGenerator
    {
        protected readonly IHashService HashService;

        public DefaultDbQueryChecksumGenerator(IHashService hashService = null)
        {
            HashService = hashService ?? new Crc32HashService();
        }

        public virtual string Generate(IDbQuery query)
        {
            Ensure.That(query, "query").IsNotNull();

            if (query.IsEmpty)
                return null;

            var sb = new StringBuilder();
            sb.AppendLine(query.Sql);
            foreach (var parameter in query.Parameters)
            {
                if(parameter is ArrayDacParameter)
                {
                    OnVisitParam((ArrayDacParameter)parameter, sb);
                    continue;
                }

                if(parameter is DacParameter)
                {
                    OnVisitParam(parameter, sb);
                    continue;
                }

                throw new SisoDbNotSupportedException(ExceptionMessages.DbQueryChecksumGenerator_UnsupportedDacParam);
            }

            return HashService.GenerateHash(sb.ToString());
        }

        protected virtual void OnVisitParam(ArrayDacParameter param, StringBuilder content)
        {
            content.Append(param.Name);
            content.Append("=[");
            var values = (object[])param.Value;
            foreach (var value in values)
            {
                content.Append(SisoEnvironment.StringConverter.AsString(value));
                content.Append(",");
            }
            content.Append("]");
        }

        protected virtual void OnVisitParam(IDacParameter param, StringBuilder content)
        {
            content.AppendFormat("{0}={1};\r\n", param.Name, SisoEnvironment.StringConverter.AsString(param.Value));
        }
    }
}