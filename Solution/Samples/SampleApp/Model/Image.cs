using System;
using System.IO;

namespace SisoDbLab.Model
{
    [Serializable]
    public class Image
    {
        private MemoryStream _content;

        public Guid SisoId { get; set; }

        public string Name { get; private set; }

        public string[] Tags { get; set; }

        public byte[] Buff
        {
            get { return _content.ToArray(); }
            private set { _content = new MemoryStream(value); }
        }

        public Image()
        {
            _content = new MemoryStream();
        }

        public void Load(string path)
        {
            Name = Path.GetFileName(path);
            Buff = File.ReadAllBytes(path);
        }
    }
}