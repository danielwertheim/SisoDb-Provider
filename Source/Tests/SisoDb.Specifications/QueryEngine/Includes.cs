using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using SisoDb.Querying;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
    namespace Includes
    {
        [Subject(typeof(IQueryEngine), "Includes using Get all as X")]
        public class when_getting_all_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
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

        [Subject(typeof(IQueryEngine), "Includes using Get all as X")]
        public class when_getting_all_using_interfaces_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
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

        [Subject(typeof(IQueryEngine), "Includes with Where, Paging and Sorting using Query as X")]
        public class when_querying_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = Establishments.SetupStructuresForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
                .QueryAs<IAlbumData, Album>(q => q
                    .Where(a => a.Name == "Born to run")
                    .SortBy(a => a.Name)
                    .Page(0, 10)
                    .Include<Genre>(a => a.GenreId)
                    .Include<Artist>(a => a.ArtistId, a => a.SecondArtistId)).ToList();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            It should_not_have_stored_genere_and_artists_in_the_json = () =>
            {
                var json = TestContext.Database.ReadOnce().GetByIdAsJson<IAlbumData>(_structure.StructureId);
                json.Length.ShouldEqual(214);
                json.ShouldNotContain("\"Genre\"");
                json.ShouldNotContain("\"Artist\"");
                json.ShouldNotContain("\"SecondArtist\"");
            };

            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Includes using Query as X")]
        public class when_querying_using_interfaces_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
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

#if Sql2008Provider
        [Subject(typeof(IQueryEngine), "Includes using Named Query")]
        public class when_named_query_including_different_firstlevel_members : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = Establishments.SetupStructuresForIncludes(TestContext);
                TestContext.DbHelper.CreateProcedure(@"create procedure [" + ProcedureName + "] as begin select s.Json,min(cs0.Json) as [GenreJson], min(cs1.Json) as [ArtistJson], min(cs2.Json) as [SecondArtistJson] from [IAlbumDataStructure] as s inner join [IAlbumDataIndexes] as si on si.[StructureId] = s.[StructureId] left join [GenreStructure] as cs0 on cs0.[StructureId] = si.[GuidValue] and si.[MemberPath]='GenreId' left join [ArtistStructure] as cs1 on cs1.[StructureId] = si.[GuidValue] and si.[MemberPath]='ArtistId' left join [ArtistStructure] as cs2 on cs2.[StructureId] = si.[GuidValue] and si.[MemberPath]='SecondArtistId' group by s.[StructureId], s.[Json] order by s.[StructureId]; end");
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

        [Subject(typeof(IQueryEngine), "Includes using Named Query")]
        public class when_named_query_using_interfaces_including_different_firstlevel_members : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = Establishments.SetupStructuresUsingInterfacesForIncludes(TestContext);
                TestContext.DbHelper.CreateProcedure(@"create procedure [" + ProcedureName + "] as begin select s.Json,min(cs0.Json) as [GenreJson], min(cs1.Json) as [ArtistJson], min(cs2.Json) as [SecondArtistJson] from [IAlbumDataStructure] as s inner join [IAlbumDataIndexes] as si on si.[StructureId] = s.[StructureId] left join [IGenreDataStructure] as cs0 on cs0.[StructureId] = si.[GuidValue] and si.[MemberPath]='GenreId' left join [IArtistDataStructure] as cs1 on cs1.[StructureId] = si.[GuidValue] and si.[MemberPath]='ArtistId' left join [IArtistDataStructure] as cs2 on cs2.[StructureId] = si.[GuidValue] and si.[MemberPath]='SecondArtistId' group by s.[StructureId], s.[Json] order by s.[StructureId]; end");
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
#endif

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
                    uow.InsertMany<Artist>(new[] { artist, secondArtist });
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
            Guid StructureId { get; set; }
            Guid? GenreId { get; }
            Guid? ArtistId { get; }
            Guid? SecondArtistId { get; }
            string Name { get; }
        }

        public interface IGenreData
        {
            Guid StructureId { get; set; }

            string Name { get; }
        }

        public interface IArtistData
        {
            Guid StructureId { get; set; }

            string Name { get; }
        }

        public class Album : IAlbumData
        {
            public Guid StructureId { get; set; }

            public Guid? GenreId
            {
                get { return Genre != null ? (Guid?)Genre.StructureId : null; }
                set { Genre.StructureId = value.Value; }
            }

            public Guid? ArtistId
            {
                get { return Artist != null ? (Guid?)Artist.StructureId : null; }
                set { Artist.StructureId = value.Value; }
            }

            public Guid? SecondArtistId
            {
                get { return SecondArtist != null ? (Guid?)SecondArtist.StructureId : null; }
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
            public Guid StructureId { get; set; }

            public string Name { get; set; }
        }

        public class Artist : IArtistData
        {
            public Guid StructureId { get; set; }

            public string Name { get; set; }
        }
    }
}