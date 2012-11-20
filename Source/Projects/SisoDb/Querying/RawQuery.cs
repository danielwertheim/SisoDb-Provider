using System;
using System.Collections.Generic;
using System.Linq;
using SisoDb.Dac;
using SisoDb.EnsureThat;

namespace SisoDb.Querying
{
	[Serializable]
	public class RawQuery : IRawQuery
	{
		private readonly IDictionary<string, IDacParameter> _parameters;

		public string QueryString { get; private set; }

		public IDacParameter[] Parameters
		{
			get { return _parameters.Values.ToArray(); }
		}

		public RawQuery(string queryString)
		{
			Ensure.That(queryString, "queryString").IsNotNullOrWhiteSpace();

			QueryString = queryString;

			_parameters = new Dictionary<string, IDacParameter>();
		}

		public void Add(params IDacParameter[] parameters)
		{
			foreach (var queryParameter in parameters)
				_parameters.Add(queryParameter.Name, queryParameter);
		}
	}
}