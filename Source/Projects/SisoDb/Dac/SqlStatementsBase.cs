using System.IO;
using System.Reflection;
using System.Resources;
using EnsureThat;

namespace SisoDb.Dac
{
    public abstract class SqlStatementsBase : ISqlStatements
    {
        private readonly ResourceManager _sqlStrings;

        protected SqlStatementsBase(Assembly assembly, string resxPath)
        {
            Ensure.That(assembly, "assembly").IsNotNull();
            Ensure.That(resxPath, "resxPath").IsNotNullOrWhiteSpace();

            var extension = Path.GetExtension(resxPath);
            if (!string.IsNullOrWhiteSpace(extension) && extension == ".resx")
                resxPath = resxPath.Substring(0, resxPath.Length - extension.Length);

            resxPath = assembly.GetName().Name + "." + resxPath;

            _sqlStrings = new ResourceManager(resxPath, assembly);
        }

        public string GetSql(string name)
        {
            return _sqlStrings.GetString(name);
        }
    }
}