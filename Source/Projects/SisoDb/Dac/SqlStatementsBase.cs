using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using EnsureThat;

namespace SisoDb.Dac
{
    public abstract class SqlStatementsBase : ISqlStatements
    {
    	private readonly Dictionary<string, string> _sqlStrings; 

        protected SqlStatementsBase(Assembly assembly, string resourcePath)
        {
            Ensure.That(assembly, "assembly").IsNotNull();
            Ensure.That(resourcePath, "resourcePath").IsNotNullOrWhiteSpace();

			if (!resourcePath.EndsWith(".resources"))
				resourcePath = string.Concat(resourcePath, ".resources");

            resourcePath = string.Concat(assembly.GetName().Name, ".", resourcePath);
            
			_sqlStrings = new Dictionary<string, string>();

			using (var resourceStream = assembly.GetManifestResourceStream(resourcePath))
			{
				using (var reader = new ResourceReader(resourceStream))
				{
					var e = reader.GetEnumerator();
					while (e.MoveNext())
					{
						_sqlStrings.Add(e.Key.ToString(), e.Value.ToString());
					}
				}

                if(resourceStream != null)
				    resourceStream.Close();
			}
        }

        public string GetSql(string name)
        {
        	return _sqlStrings[name];
        }
    }
}