namespace SisoDb.Diagnostics
{
    public static class DiagnosticsExtensions
    {
         public static DiagnosticsSection GetDiagnostics(this ISisoDatabase database)
         {
             return new DbDiagnosticsSectionBuilder(database).Build();
         }
    }
}