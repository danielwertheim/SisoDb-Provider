using System.Linq;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Querying;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.Queries
{
    [TestFixture]
    public class SqlUnitOfWorkNamedQueryIncludeTests : SqlIntegrationTestBase
    {
        private const string ProcedureName = "NamedQueryIncludeTest";
        private const string ProcedureNameForInterfaces = "NamedQueryIncludeTestInterfaces";

        protected override void OnFixtureInitialize()
        {
            DeleteProcedures();
            CreateStoredProcedure();
            CreateStoredProcedureForInterfaces();
        }

        protected override void OnFixtureFinalize()
        {
            DeleteProcedures();
        }

        protected override void OnTestFinalize()
        {
            DropStructureSet<IAlbumData>();
            DropStructureSet<IGenreData>();
            DropStructureSet<IArtistData>();
            DropStructureSet<Genre>();
            DropStructureSet<Artist>();
        }

        private void CreateStoredProcedure()
        {
            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();
            var hashForName = memberNameGenerator.Generate("Name");
            var hashForGenreId = memberNameGenerator.Generate("GenreId");
            var hashForArtistId = memberNameGenerator.Generate("ArtistId");
            var hashForSecondArtistId = memberNameGenerator.Generate("SecondArtistId");

            var sql =
                string.Format("create procedure [dbo].[{0}] as begin "
                + "select Json, "
                + "(select cs1.Json from dbo.GenreStructure as cs1 where cs1.SisoId = I.{1}) as Genre, "
                + "(select cs2.Json from dbo.ArtistStructure as cs2 where cs2.SisoId = I.{2}) as Artist, "
                + "(select cs3.Json from dbo.ArtistStructure as cs3 where cs3.SisoId = I.{3}) as SecondArtist "
                + "from dbo.IAlbumDataStructure as S inner join dbo.IAlbumDataIndexes as I on I.SisoId = S.SisoId "
                + "order by I.[{4}];"
                + "end", ProcedureName, hashForGenreId, hashForArtistId, hashForSecondArtistId, hashForName);

            DbHelper.CreateProcedure(sql);
        }

        private void CreateStoredProcedureForInterfaces()
        {
            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();
            var hashForName = memberNameGenerator.Generate("Name");
            var hashForGenreId = memberNameGenerator.Generate("GenreId");
            var hashForArtistId = memberNameGenerator.Generate("ArtistId");
            var hashForSecondArtistId = memberNameGenerator.Generate("SecondArtistId");

            var sql =
                string.Format("create procedure [dbo].[{0}] as begin "
                + "select Json, "
                + "(select cs1.Json from dbo.IGenreDataStructure as cs1 where cs1.SisoId = I.{1}) as Genre, "
                + "(select cs2.Json from dbo.IArtistDataStructure as cs2 where cs2.SisoId = I.{2}) as Artist, "
                + "(select cs3.Json from dbo.IArtistDataStructure as cs3 where cs3.SisoId = I.{3}) as SecondArtist "
                + "from dbo.IAlbumDataStructure as S inner join dbo.IAlbumDataIndexes as I on I.SisoId = S.SisoId "
                + "order by I.[{4}];"
                + "end", ProcedureNameForInterfaces, hashForGenreId, hashForArtistId, hashForSecondArtistId, hashForName);

            DbHelper.CreateProcedure(sql);
        }

        private void DeleteProcedures()
        {
            DbHelper.DropProcedure("[dbo].[{0}]".Inject(ProcedureName));
            DbHelper.DropProcedure("[dbo].[{0}]".Inject(ProcedureNameForInterfaces));
        }

        [Test]
        public void NamedQuery_WhenIncludingDifferentFirstLevelMembers_CompleteStructureIsRefetched()
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

                var query = new NamedQuery(ProcedureName);
                refetched = uow.NamedQuery<Album>(query).Single();
            }

            AssertAlbumEquality(album, refetched);
        }

        [Test]
        public void NamedQuery_WhenUsingInterfacesAndIncludingDifferentFirstLevelMembers_CompleteStructureIsRefetched()
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

                var query = new NamedQuery(ProcedureNameForInterfaces);
                refetched = uow.NamedQuery<Album>(query).Single();
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