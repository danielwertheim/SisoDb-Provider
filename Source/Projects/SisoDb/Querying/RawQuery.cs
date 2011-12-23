using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EnsureThat;
using SisoDb.Dac;

namespace SisoDb.Querying
{
	[Serializable]
	public class RawQuery : IRawQuery
	{
		private readonly IDictionary<string, IDacParameter> _parameters;

		public string QueryString { get; private set; }

		public IEnumerable<IDacParameter> Parameters
		{
			get { return new ReadOnlyCollection<IDacParameter>(_parameters.Values.ToList()); }
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