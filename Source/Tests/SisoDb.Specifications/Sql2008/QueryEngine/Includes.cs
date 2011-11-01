using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Querying;
using SisoDb.Sql2008;
using SisoDb.Testing;

namespace SisoDb.Specifications.Sql2008.QueryEngine
{
    namespace Includes
    {
        [Subject(typeof(Sql2008QueryEngine), "Includes using Get all as X")]
        public class when_getting_all_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structure = Establishments.SetupStructuresForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
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

        [Subject(typeof(Sql2008QueryEngine), "Includes using Get all as X")]
        public class when_getting_all_using_interfaces_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structure = Establishments.SetupStructuresUsingInterfacesForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                .GetAllAs<IAlbumData, Album>(q => q
                    .Include<IGenreData>(a => a.GenreId)
                    .Include<IArtistData>(a => a.ArtistId, a => a.SecondArtistId)).ToList();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(Sql2008QueryEngine), "Includes using Query as X")]
        public class when_querying_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structure = Establishments.SetupStructuresForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                .QueryAs<IAlbumData, Album>(q => q
                    .Include<Genre>(a => a.GenreId)
                    .Include<Artist>(a => a.ArtistId, a => a.SecondArtistId)).ToList();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(Sql2008QueryEngine), "Includes using Query as X")]
        public class when_querying_using_interfaces_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structure = Establishments.SetupStructuresUsingInterfacesForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                .QueryAs<IAlbumData, Album>(q => q
                    .Include<IGenreData>(a => a.GenreId)
                    .Include<IArtistData>(a => a.ArtistId, a => a.SecondArtistId)).ToList();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(Sql2008QueryEngine), "Includes using Named Query")]
        public class when_named_query_including_different_firstlevel_members : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structure = Establishments.SetupStructuresForIncludes(TestContext);
                TestContext.DbHelper.CreateProcedure(@"create procedure [dbo].[" + ProcedureName + "] as begin select s.Json,min(cs0.Json) as [Genre], min(cs1.Json) as [Artist], min(cs2.Json) as [SecondArtist] from [dbo].[IAlbumDataStructure] as s inner join [dbo].[IAlbumDataIndexes] as si on si.[StructureId] = s.[StructureId] left join [dbo].[GenreStructure] as cs0 on cs0.[StructureId] = si.[IntegerValue] and si.[MemberPath]='GenreId' left join [dbo].[ArtistStructure] as cs1 on cs1.[StructureId] = si.[IntegerValue] and si.[MemberPath]='ArtistId' left join [dbo].[ArtistStructure] as cs2 on cs2.[StructureId] = si.[IntegerValue] and si.[MemberPath]='SecondArtistId' group by s.[StructureId], s.[Json] order by s.[StructureId]; end");
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of =
                () => _fetchedStructures = TestContext.Database.ReadOnce().NamedQueryAs<IAlbumData, Album>(new NamedQuery(ProcedureName)).ToList();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            private const string ProcedureName = "NamedQueryIncludeTest";
            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(Sql2008QueryEngine), "Includes using Named Query")]
        public class when_named_query_using_interfaces_including_different_firstlevel_members : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create(StorageProviders.Sql2008);
                _structure = Establishments.SetupStructuresUsingInterfacesForIncludes(TestContext);
                TestContext.DbHelper.CreateProcedure(@"create procedure [dbo].[" + ProcedureName + "] as begin select s.Json,min(cs0.Json) as [Genre], min(cs1.Json) as [Artist], min(cs2.Json) as [SecondArtist] from [dbo].[IAlbumDataStructure] as s inner join [dbo].[IAlbumDataIndexes] as si on si.[StructureId] = s.[StructureId] left join [dbo].[IGenreDataStructure] as cs0 on cs0.[StructureId] = si.[IntegerValue] and si.[MemberPath]='GenreId' left join [dbo].[IArtistDataStructure] as cs1 on cs1.[StructureId] = si.[IntegerValue] and si.[MemberPath]='ArtistId' left join [dbo].[IArtistDataStructure] as cs2 on cs2.[StructureId] = si.[IntegerValue] and si.[MemberPath]='SecondArtistId' group by s.[StructureId], s.[Json] order by s.[StructureId]; end");
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of =
                () => _fetchedStructures = TestContext.Database.ReadOnce().NamedQueryAs<IAlbumData, Album>(new NamedQuery(ProcedureName)).ToList();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            private const string ProcedureName = "NamedQueryIncludeTestInterfaces";
            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        internal static class Establishments
        {
            internal static Album SetupStructuresForIncludes(ITestContext testContext)
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

                testContext.Database.WithUnitOfWork(uow =>
                {
                    uow.Insert(genre);
                    uow.InsertMany(new[] { artist, secondArtist });
                    uow.Insert<IAlbumData>(album);
                    uow.Commit();
                });

                return album;
            }

            internal static Album SetupStructuresUsingInterfacesForIncludes(ITestContext testContext)
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

                testContext.Database.WithUnitOfWork(uow =>
                {
                    uow.Insert<IGenreData>(genre);
                    uow.InsertMany<IArtistData>(new[] { artist, secondArtist });
                    uow.Insert<IAlbumData>(album);
                    uow.Commit();
                });

                return album;
            }
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