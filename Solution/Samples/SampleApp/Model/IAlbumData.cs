using System;

namespace SisoDbLab.Model
{
    public interface IAlbumData
    {
        Guid SisoId { get; set; }
        Guid GenreId { get; set; }
        Guid ArtistId { get; set; }
        string Name { get; set; }
    }
}