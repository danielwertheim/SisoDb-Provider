using System;
using System.Collections.Generic;
using Machine.Specifications;
using SisoDb.Querying;
using SisoDb.Testing;

namespace SisoDb.Specifications.QueryEngine
{
	class Includes
    {
        [Subject(typeof(IReadSession), "Includes using Get all as X")]
        public class when_getting_all_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = Establishments.SetupStructuresForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
				.Query<IAlbumData>()
                .Include<Genre>(a => a.GenreId)
                .Include<Artist>(a => a.ArtistId, a => a.SecondArtistId).ToListOf<Album>();

            It should_have_fetched_2_albums = 
                () => _fetchedStructures.Count.ShouldEqual(2);

            It should_have_fetched_albums_correctly = () =>
            {
            	_fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);
				_fetchedStructures[1].ShouldBeValueEqualTo(_structures[1]);
			};

			private static IList<Album> _structures;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(IReadSession), "Includes using Get all as X")]
        public class when_getting_all_using_interfaces_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = Establishments.SetupStructuresUsingInterfacesForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
				.Query<IAlbumData>()
				.Include<IGenreData>(a => a.GenreId)
                .Include<IArtistData>(a => a.ArtistId, a => a.SecondArtistId).ToListOf<Album>();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structure);

            private static Album _structure;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(IReadSession), "Includes with Where, Paging and Sorting using Query as X")]
        public class when_querying_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structures = Establishments.SetupStructuresForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
				.Query<IAlbumData>()
                .Where(a => a.Name == "Born to run")
                .OrderBy(a => a.Name)
                .Page(0, 10)
                .Include<Genre>(a => a.GenreId)
                .Include<Artist>(a => a.ArtistId, a => a.SecondArtistId).ToListOf<Album>();

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);

            It should_not_have_stored_genere_and_artists_in_the_json = () =>
            {
                var json = TestContext.Database.ReadOnce().GetByIdAsJson<IAlbumData>(_structures[0].StructureId);
                json.Length.ShouldEqual(214);
                json.ShouldNotContain("\"Genre\"");
                json.ShouldNotContain("\"Artist\"");
                json.ShouldNotContain("\"SecondArtist\"");
            };

			private static IList<Album> _structures;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(IReadSession), "Includes using Query as X")]
        public class when_querying_using_interfaces_and_including_different_firstlevel_members : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = Establishments.SetupStructuresUsingInterfacesForIncludes(TestContext);
            };

            Because of = () => _fetchedStructures = TestContext.Database.ReadOnce()
				.Query<IAlbumData>()
                .Include<IGenreData>(a => a.GenreId)
                .Include<IArtistData>(a => a.ArtistId, a => a.SecondArtistId).ToListOf<Album>();

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
                _structures = Establishments.SetupStructuresForIncludes(TestContext);
                TestContext.DbHelper.CreateProcedure(@"create procedure [" + ProcedureName + "] as begin select s.Json,min(cs0.Json) as [GenreJson], min(cs1.Json) as [ArtistJson], min(cs2.Json) as [SecondArtistJson] from [IAlbumDataStructure] as s inner join [IAlbumDataStrings] mem0 on mem0.StructureId = s.StructureId and mem0.MemberPath = 'Name' inner join [IAlbumDataGuids] as si on si.[StructureId] = s.[StructureId] left join [GenreStructure] as cs0 on cs0.[StructureId] = si.[Value] and si.[MemberPath]='GenreId' left join [ArtistStructure] as cs1 on cs1.[StructureId] = si.[Value] and si.[MemberPath]='ArtistId' left join [ArtistStructure] as cs2 on cs2.[StructureId] = si.[Value] and si.[MemberPath]='SecondArtistId' where mem0.Value = 'Born to run' group by s.[StructureId], s.[Json] order by s.[StructureId]; end");
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of =
                () => _fetchedStructures = TestContext.Database.ReadOnce().NamedQueryAs<IAlbumData, Album>(new NamedQuery(ProcedureName));

            It should_have_fetched_1_album =
                () => _fetchedStructures.Count.ShouldEqual(1);

            It should_have_fetched_album =
                () => _fetchedStructures[0].ShouldBeValueEqualTo(_structures[0]);

            private const string ProcedureName = "NamedQueryIncludeTest";
			private static IList<Album> _structures;
            private static IList<Album> _fetchedStructures;
        }

        [Subject(typeof(IQueryEngine), "Includes using Named Query")]
        public class when_named_query_using_interfaces_including_different_firstlevel_members : SpecificationBase, ICleanupAfterEveryContextInAssembly
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _structure = Establishments.SetupStructuresUsingInterfacesForIncludes(TestContext);
                TestContext.DbHelper.CreateProcedure(@"create procedure [" + ProcedureName + "] as begin select s.Json,min(cs0.Json) as [GenreJson], min(cs1.Json) as [ArtistJson], min(cs2.Json) as [SecondArtistJson] from [IAlbumDataStructure] as s inner join [IAlbumDataStrings] mem0 on mem0.StructureId = s.StructureId and mem0.MemberPath = 'Name' inner join [IAlbumDataGuids] as si on si.[StructureId] = s.[StructureId] left join [IGenreDataStructure] as cs0 on cs0.[StructureId] = si.[Value] and si.[MemberPath]='GenreId' left join [IArtistDataStructure] as cs1 on cs1.[StructureId] = si.[Value] and si.[MemberPath]='ArtistId' left join [IArtistDataStructure] as cs2 on cs2.[StructureId] = si.[Value] and si.[MemberPath]='SecondArtistId' where mem0.Value = 'Born to run' group by s.[StructureId], s.[Json] order by s.[StructureId]; end");
            };

            public void AfterContextCleanup()
            {
                TestContext.DbHelper.DropProcedure(ProcedureName);
            }

            Because of =
                () => _fetchedStructures = TestContext.Database.ReadOnce().NamedQueryAs<IAlbumData, Album>(new NamedQuery(ProcedureName));

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
            internal static Album[] SetupStructuresForIncludes(ITestContext testContext)
            {
                var genre = new Genre { Name = "Rock" };
            	var secondGenre = new Genre {Name = "Pop"};

                var artist = new Artist { Name = "Bruce" };
                var secondArtist = new Artist { Name = "e-street" };
				var thirdArtist = new Artist { Name = "Foo artist" };
                
				var album = new Album
                {
                    Name = "Born to run",
                    Genre = genre,
                    Artist = artist,
                    SecondArtist = secondArtist
                };
				var secondAlbum = new Album
				{
					Name = "Born to run (pop version)",
					Genre = secondGenre,
					Artist = artist,
					SecondArtist = secondArtist
				};

                testContext.Database.WithWriteSession(session =>
                {
                    session.InsertMany(new [] { genre, secondGenre });
                    session.InsertMany(new [] { artist, secondArtist, thirdArtist });
                    session.InsertMany<IAlbumData>(new [] { album, secondAlbum });
                });

                return new [] { album, secondAlbum };
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

                testContext.Database.WithWriteSession(session =>
                {
                    session.Insert<IGenreData>(genre);
                    session.InsertMany<IArtistData>(new[] { artist, secondArtist });
                    session.Insert<IAlbumData>(album);
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