using System;

namespace SisoDbLab.Model
{
    public class Album : IAlbumData
    {
        public Guid SisoId { get; set; }

        public Guid GenreId
        {
            get { return Genre.SisoId; }
            set { Genre.SisoId = value; }
        }

        public Guid ArtistId
        {
            get { return Artist.SisoId; }
            set { Artist.SisoId = value; }
        }

        public string Name { get; set; }

        public Genre Genre { get; set; }

        public Artist Artist { get; set; }

        public Album()
        {
            Genre = new Genre();
            Artist = new Artist();
        }
    }
}