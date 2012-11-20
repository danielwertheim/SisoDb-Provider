using SisoDb.Dac;

namespace SisoDb
{
	public interface IRawQuery
	{
		string QueryString { get; }
        IDacParameter[] Parameters { get; }

		void Add(params IDacParameter[] parameters);
	}
}