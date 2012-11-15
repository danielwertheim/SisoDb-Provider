using System.Linq;
using Machine.Specifications;
using SisoDb.Spatial;
using SisoDb.Specifications.Model;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Spatial
{
#if Sql2008Provider || Sql2012Provider
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
                }
                _coordinates = SpatialDataFactory.DefaultPolygon();
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
                }
                _coordinates = SpatialDataFactory.DefaultPolygon();
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
        }

        [Subject(typeof(ISisoSpatial), "DeleteGeoIn")]
        public class when_deleting_existing_polygon : SpecificationBase
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
                    s.InsertPolygonTo<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());

                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.DeleteGeoIn<SpatialGuidItem>(_item.StructureId);
                }
            };

            It should_have_no_geo_record_left =
                () => TestContext.DbHelper.should_have_been_deleted_from_spatial_table(_structureSchema, _item.StructureId);

            It should_now_return_empty_array_of_coordinates = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(new Coordinates[0]);
                }
            };

            private static SpatialGuidItem _item;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "DeleteGeoIn")]
        public class when_deleting_non_existing_polygon : SpecificationBase
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
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.DeleteGeoIn<SpatialGuidItem>(_item.StructureId);
                }
            };

            It should_have_no_geo_record_left =
                () => TestContext.DbHelper.should_have_been_deleted_from_spatial_table(_structureSchema, _item.StructureId);

            It should_now_return_empty_array_of_coordinates = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(new Coordinates[0]);
                }
            };

            private static SpatialGuidItem _item;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "SetPolygonIn")]
        public class when_setting_polygon_and_one_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                var orgCoordinates = SpatialDataFactory.DefaultPolygon();
                _newCoordinates = SpatialDataFactory.SmallerPolygon();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygonIn<SpatialGuidItem>(_item.StructureId, orgCoordinates);
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
        }

        [Subject(typeof(ISisoSpatial), "UpdatePolygonIn")]
        public class when_updating_an_existing_polygon : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                var orgCoordinates = SpatialDataFactory.DefaultPolygon();
                _newCoordinates = SpatialDataFactory.SmallerPolygon();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygonIn<SpatialGuidItem>(_item.StructureId, orgCoordinates);
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
        }

        [Subject(typeof(ISisoSpatial), "PolygonContainsPointFor")]
        public class when_checking_all_points_of_polygon_for_existance : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _coordinates = SpatialDataFactory.DefaultPolygon();
                _exists = new bool[_coordinates.Length];
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygonTo<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    for (var i = 0; i < _coordinates.Length; i++)
                        _exists[i] = s.PolygonContainsPointFor<SpatialGuidItem>(_item.StructureId, _coordinates[i]);
                }
            };

            It should_have_responded_false_for_all_points_of_the_polygon = 
                () => _exists.All(e => e).ShouldBeFalse();

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
            private static bool[] _exists;
        }

        [Subject(typeof(ISisoSpatial), "PolygonContainsPointFor")]
        public class when_checking_if_polygon_contains_point_that_is_within_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _coordinates = SpatialDataFactory.DefaultPolygon();
                _exists = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygonTo<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    _exists = s.PolygonContainsPointFor<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.PointWithinDefaultPolygon);
                }
            };

            It should_be_true =
                () => _exists.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
            private static bool _exists;
        }
    }
#endif
}
