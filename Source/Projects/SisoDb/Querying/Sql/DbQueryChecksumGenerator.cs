using System.Text;
using SisoDb.Dac;
using SisoDb.EnsureThat;
using SisoDb.NCore.Cryptography;
using SisoDb.Resources;

namespace SisoDb.Querying.Sql
{
    public class DbQueryChecksumGenerator
    {
        protected readonly IHashService HashService;

        public DbQueryChecksumGenerator()
        {
            HashService = new Crc32HashService();
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
                    sb.Append(parameter.Name);
                    sb.Append("=[");
                    var values = (object[])((ArrayDacParameter)parameter).Value;
                    foreach (var value in values)
                    {
                        sb.Append(SisoEnvironment.StringConverter.AsString(value));
                        sb.Append(",");
                    }
                    sb.Append("]");
                    continue;
                }

                if(parameter is DacParameter)
                {
                    sb.AppendFormat("{0}={1};\r\n", parameter.Name, SisoEnvironment.StringConverter.AsString(parameter.Value));
                    continue;
                }

                throw new SisoDbNotSupportedException(ExceptionMessages.DbQueryChecksumGenerator_UnsupportedDacParam);
            }

            return HashService.GenerateHash(sb.ToString());
        }
    }
}