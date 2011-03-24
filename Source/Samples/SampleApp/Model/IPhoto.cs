using System;

namespace SisoDbLab.Model
{
    public interface IPhoto
    {
        Guid SisoId { get; set; }

        string Name { get; }

        string Path { get; }
    }
}