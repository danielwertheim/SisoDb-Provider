using System;

namespace SisoDbLab.Model
{
    public interface IPhoto
    {
        Guid Id { get; set; }

        string Name { get; }

        string Path { get; }
    }
}