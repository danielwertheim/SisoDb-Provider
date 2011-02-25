using System;

namespace SisoDbLab.Model
{
    public class Album : IAlbumData
    {
        public Guid Id { get; set; }

        public Guid GenreId
        {
            get { return Genre.Id; }
            set { Genre.Id = value; }
        }

        public Guid ArtistId
        {
            get { return Artist.Id; }
            set { Artist.Id = value; }
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