namespace SisoDb.Querying
{
    public interface ICommandBuilderFactory
    {
        IGetCommandBuilder<T> CreateGetCommandBuilder<T>() where T : class;
        IQueryCommandBuilder<T> CreateQueryCommandBuilder<T>() where T : class;
    }
}