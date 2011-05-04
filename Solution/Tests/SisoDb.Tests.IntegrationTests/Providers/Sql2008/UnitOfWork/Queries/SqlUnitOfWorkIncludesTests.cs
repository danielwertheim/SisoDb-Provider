using System.Linq;
using NUnit.Framework;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkIncludesTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<IAlbumData>();
            DropStructureSet<IGenreData>();
            DropStructureSet<IArtistData>();
            DropStructureSet<Genre>();
            DropStructureSet<Artist>();
        }

        [Test]
        public void GetAllAs_WhenIncludingDifferentFirstLevelMembers_CompleteStructureIsRefetched()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Bruce" };
            var secondArtist = new Artist { Name = "e-street" };
            var album = new Album
            {
                Name = "Born to run",
                Genre = genre,
                Artist = artist,
                SecondArtist = secondArtist
            };

            Album refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(genre);
                uow.InsertMany(new[] { artist, secondArtist });
                uow.Insert<IAlbumData>(album);
                uow.Commit();

                refetched = uow.GetAllAs<IAlbumData, Album>(q => q
                    .Include<Genre>(a => a.GenreId)
                    .Include<Artist>(
                        a => a.ArtistId,
                        a => a.SecondArtistId)).Single();
            }

            AssertAlbumEquality(album, refetched);
        }

        [Test]
        public void GetAllAs_WhenUsingInterfacesAndIncludingDifferentFirstLevelMembers_CompleteStructureIsRefetched()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Bruce" };
            var secondArtist = new Artist { Name = "e-street" };
            var album = new Album
            {
                Name = "Born to run",
                Genre = genre,
                Artist = artist,
                SecondArtist = secondArtist
            };

            Album refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert<IGenreData>(genre);
                uow.InsertMany<IArtistData>(new [] { artist, secondArtist });
                uow.Insert<IAlbumData>(album);
                uow.Commit();

                refetched = uow.GetAllAs<IAlbumData, Album>(q => q
                    .Include<IGenreData>(a => a.GenreId)
                    .Include<IArtistData>(
                        a => a.ArtistId,
                        a => a.SecondArtistId)).Single();
            }

            AssertAlbumEquality(album, refetched);
        }

        [Test]
        public void QueryAs_WhenIncludingDifferentFirstLevelMembers_CompleteStructureIsRefetched()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Bruce" };
            var secondArtist = new Artist {Name = "e-street"};
            var album = new Album
            {
                Name = "Born to run", 
                Genre = genre, 
                Artist = artist, 
                SecondArtist = secondArtist
            };

            Album refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(genre);
                uow.InsertMany(new []{artist, secondArtist});
                uow.Insert<IAlbumData>(album);
                uow.Commit();
                
                refetched = uow.QueryAs<IAlbumData, Album>(q => q
                    .Include<Genre>(a => a.GenreId)
                    .Include<Artist>(
                        a => a.ArtistId, 
                        a => a.SecondArtistId)).Single();
            }

            AssertAlbumEquality(album, refetched);
        }

        [Test]
        public void QueryAs_WhenUsingInterfacesAndIncludingDifferentFirstLevelMembers_CompleteStructureIsRefetched()
        {
            var genre = new Genre { Name = "Rock" };
            var artist = new Artist { Name = "Bruce" };
            var secondArtist = new Artist { Name = "e-street" };
            var album = new Album
            {
                Name = "Born to run",
                Genre = genre,
                Artist = artist,
                SecondArtist = secondArtist
            };

            Album refetched;
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert<IGenreData>(genre);
                uow.InsertMany<IArtistData>(new[] { artist, secondArtist });
                uow.Insert<IAlbumData>(album);
                uow.Commit();

                refetched = uow.QueryAs<IAlbumData, Album>(q => q
                    .Include<IGenreData>(a => a.GenreId)
                    .Include<IArtistData>(a => a.ArtistId, a => a.SecondArtistId))
                    .Single();
            }

            AssertAlbumEquality(album, refetched);
        }

        private static void AssertAlbumEquality(Album expectedAlbum, Album actualAlbum)
        {
            CustomAssert.AreValueEqual(expectedAlbum, actualAlbum);
            CustomAssert.AreValueEqual(expectedAlbum.Genre, actualAlbum.Genre);
            CustomAssert.AreValueEqual(expectedAlbum.Artist, actualAlbum.Artist);
            CustomAssert.AreValueEqual(expectedAlbum.SecondArtist, actualAlbum.SecondArtist);
        }

        private interface IAlbumData
        {
            int SisoId { get; set; }
            int? GenreId { get; }
            int? ArtistId { get; }
            int? SecondArtistId { get; }
            string Name { get; }
        }

        private interface IGenreData
        {
            int SisoId { get; set; }

            string Name { get; }
        }

        private interface IArtistData
        {
            int SisoId { get; set; }

            string Name { get; }
        }

        private class Album : IAlbumData
        {
            public int SisoId { get; set; }

            public int? GenreId
            {
                get { return Genre != null ? (int?)Genre.SisoId : null; }
                set { Genre.SisoId = value.Value; }
            }

            public int? ArtistId
            {
                get { return Artist != null ? (int?)Artist.SisoId : null; }
                set { Artist.SisoId = value.Value; }
            }

            public int? SecondArtistId
            {
                get { return SecondArtist != null ? (int?)SecondArtist.SisoId : null; }
                set { SecondArtist.SisoId = value.Value; }
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

        private class Genre : IGenreData
        {
            public int SisoId { get; set; }

            public string Name { get; set; }
        }

        private class Artist : IArtistData
        {
            public int SisoId { get; set; }

            public string Name { get; set; }
        }
    }
}