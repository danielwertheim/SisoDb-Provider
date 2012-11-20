using SisoDb.Dac;

namespace SisoDb
{
    public interface INamedQuery
    {
        string Name { get; }
        IDacParameter[] Parameters { get; }

        void Add(params IDacParameter[] parameters);
    }
}