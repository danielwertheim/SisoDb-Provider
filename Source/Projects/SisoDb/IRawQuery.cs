using System.Collections.Generic;
using SisoDb.Dac;

namespace SisoDb
{
	public interface IRawQuery
	{
		string QueryString { get; }

		IEnumerable<IDacParameter> Parameters { get; }

		void Add(params IDacParameter[] parameters);
	}
}