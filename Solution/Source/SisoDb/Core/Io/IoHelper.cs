using System.IO;

namespace SisoDb.Core.Io
{
    internal static class IoHelper
    {
        internal static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        internal static void DeleteIfFileExists(string path)
        {
            if(!FileExists(path))
                return;

            File.Delete(path);
        }
    }
}