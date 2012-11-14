using System.Linq;
using Machine.Specifications;
using SisoDb.Spatial;
using SisoDb.Specifications.Model;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Spatial
{
    class GeographicalOperations
    {
        [Subject(typeof(ISisoSpatial), "SetPolygonIn")]
        public class when_setting_polygon_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();

                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
                _coordinates = new[] 
                {
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.649, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                };
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.SetPolygonIn<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "InsertPolygonTo")]
        public class when_inserting_polygon_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();

                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
                _coordinates = new[] 
                {
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.649, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                };
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.InsertPolygonTo<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "SetPolygonIn")]
        public class when_setting_polygon_and_one_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                var orgCoordinates = new[] 
                {
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.649, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                };
                _newCoordinates = orgCoordinates.Select((c, i) => i == 3 ? null : c).Where(c => c != null).ToArray();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygonIn<SpatialGuidItem>(_item.StructureId, orgCoordinates);
                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.SetPolygonIn<SpatialGuidItem>(_item.StructureId, _newCoordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _newCoordinates;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "UpdatePolygonIn")]
        public class when_updating_an_existing_polygon : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                var orgCoordinates = new[] 
                {
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.649, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.348 },
                    new Coordinates { Latitude = 47.658, Longitude = 122.358 },
                    new Coordinates { Latitude = 47.653, Longitude = 122.358 },
                };
                _newCoordinates = orgCoordinates.Select((c, i) => i == 3 ? null : c).Where(c => c != null).ToArray();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygonIn<SpatialGuidItem>(_item.StructureId, orgCoordinates);
                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.UpdatePolygonIn<SpatialGuidItem>(_item.StructureId, _newCoordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _newCoordinates;
            private static IStructureSchema _structureSchema;
        }
    }
}
