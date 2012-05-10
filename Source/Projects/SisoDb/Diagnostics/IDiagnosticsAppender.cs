namespace SisoDb.Diagnostics
{
    public interface IDiagnosticsAppender<in T> 
    {
        void Append(T info);
    }
}