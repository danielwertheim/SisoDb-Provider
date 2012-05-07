namespace SisoDb.Diagnostics
{
    public interface IDiagnosticsContextAppender<in T> 
    {
        void Append(T info);
    }
}