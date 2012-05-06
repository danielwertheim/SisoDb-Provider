namespace SisoDb.Diagnostics
{
    public static class DiagnosticsExtensions
    {
         public static DiagnosticsContext GetDiagnostics(this ISisoDatabase database)
         {
             return new DbDiagnosticsContextBuilder(database).Build();
         }
    }
}