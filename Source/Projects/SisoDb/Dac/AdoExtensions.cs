using System.Data;

namespace SisoDb.Dac
{
    public static class AdoExtensions
    {
        public static IDataReader SingleResultSequentialReader(this IDbCommand cmd)
        {
            return cmd.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SequentialAccess);
        }
    }
}