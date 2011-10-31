using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    namespace Includes
    {
        [Subject(typeof(Sql2008QueryEngine), "Includes using Get all as X")]
        public class when_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);

                var genre = new Genre { Name = "Rock" };
                var artist = new Artist { Name = "Bruce" };
                var secondArtist = new Artist { Name = "e-street" };
                _structure = new Album
                {
                    Name = "Born to run",
                    Genre = genre,
                    Artist = artist,
                    SecondArtist = secondArtist
                };

                TestContext.Database.WithUnitOfWork(uow =>
                {
                    uow.Insert(genre);
                    uow.InsertMany(new[]{artist, secondArtist});
                    uow.Insert<IAlbumData>(_structure);
                    uow.Commit();
                });
            };

            Because of = () => _fetchedStructures = TestContext.Database.FetchVia()
                .GetAllAs<IAlbumData, Album>(q => q
                    .Include<Genre>(a => a.GenreId)
                    .Include<Artist>(a => a.ArtistId, a => a.SecondArtistId)).ToList();

            It should_have_fetched_1_album = 
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album = 
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        public interface IAlbumData
        {
            int StructureId { get; set; }
            int? GenreId { get; }
            int? ArtistId { get; }
            int? SecondArtistId { get; }
            string Name { get; }
        }

        public interface IGenreData
        {
            int StructureId { get; set; }

            string Name { get; }
        }

        public interface IArtistData
        {
            int StructureId { get; set; }

            string Name { get; }
        }

        public class Album : IAlbumData
        {
            public int StructureId { get; set; }

            public int? GenreId
            {
                get { return Genre != null ? (int?)Genre.StructureId : null; }
                set { Genre.StructureId = value.Value; }
            }

            public int? ArtistId
            {
                get { return Artist != null ? (int?)Artist.StructureId : null; }
                set { Artist.StructureId = value.Value; }
            }

            public int? SecondArtistId
            {
                get { return SecondArtist != null ? (int?)SecondArtist.StructureId : null; }
                set { SecondArtist.StructureId = value.Value; }
            }

            public string Name { get; set; }

            public Genre Genre { get; set; }

            public Artist Artist { get; set; }

            public Artist SecondArtist { get; set; }

            public Album()
            {
                Genre = new Genre();
                Artist = new Artist();
                SecondArtist = new Artist();
            }
        }

        public class Genre : IGenreData
        {
            public int StructureId { get; set; }

            public string Name { get; set; }
        }

        public class Artist : IArtistData
        {
            public int StructureId { get; set; }

            public string Name { get; set; }
        }
    }
}