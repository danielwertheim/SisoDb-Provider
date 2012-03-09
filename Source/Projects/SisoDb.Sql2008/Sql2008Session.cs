namespace SisoDb.Sql2008
{
    public class Sql2008Session : DbSession
    {
        internal Sql2008Session(ISisoDbDatabase db)
			: base(db)
        {}
    }
}