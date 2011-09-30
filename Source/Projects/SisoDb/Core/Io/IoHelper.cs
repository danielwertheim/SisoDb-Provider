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

        internal static void CreateEmptyFile(string filePath)
        {
            using (var f = File.Open(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                f.Close();
            }
        }
    }
}