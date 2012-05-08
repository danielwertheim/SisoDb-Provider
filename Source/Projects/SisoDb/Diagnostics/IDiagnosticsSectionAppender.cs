namespace SisoDb.Diagnostics
{
    public interface IDiagnosticsSectionAppender<in T> 
    {
        void Append(T info);
    }
}