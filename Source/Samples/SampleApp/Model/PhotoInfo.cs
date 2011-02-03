using System;

namespace SisoDbLab.Model
{
    public class PhotoInfo : IPhoto
    {
        public Guid Id { get; set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
    }
}