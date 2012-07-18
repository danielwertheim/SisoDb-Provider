using System.IO;

namespace NCore.Io
{
    public static class IoUtils
    {
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static void DeleteIfFileExists(string path)
        {
            if (!FileExists(path))
                return;

            File.Delete(path);
        }

        public static void CreateEmptyFile(string filePath)
        {
            using (var f = File.Open(filePath, FileMode.CreateNew, FileAccess.Write))
            {
                f.Close();
            }
        }
    }
}