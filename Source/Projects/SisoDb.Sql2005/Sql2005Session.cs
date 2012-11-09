namespace SisoDb.Sql2005
{
    public class Sql2005Session : DbSession
    {
        internal Sql2005Session(ISisoDatabase db) : base(db)
        {}
    }
}