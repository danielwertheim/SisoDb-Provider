using System;
using System.IO;

namespace SisoDbLab.Model
{
    public class Photo : IPhoto
    {
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public string Path { get; private set; }

        public MemoryStream Stream { get; private set; }

        public Photo()
        {
            Stream = new MemoryStream();
        }

        public void Load(string path)
        {
            Name = System.IO.Path.GetFileName(path);
            Path = path;
            Stream = new MemoryStream(File.ReadAllBytes(path));
        }
    }
}