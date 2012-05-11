using SisoDb.Diagnostics.Builders;

namespace SisoDb.Diagnostics
{
    public static class DiagnosticsExtensions
    {
         public static DiagnosticsInfo GetDiagnostics(this ISisoDatabase database)
         {
             return new DbDiagnosticsBuilder(database).Build();
         }
    }
}