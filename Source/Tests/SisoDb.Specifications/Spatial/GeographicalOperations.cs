using System.Linq;
using Machine.Specifications;
using SisoDb.NCore;
using SisoDb.Spatial;
using SisoDb.Spatial.Resources;
using SisoDb.Specifications.Model;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Spatial
{
#if SqlAzureProvider || Sql2008Provider || Sql2012Provider
    class GeographicalOperations
    {
        [Subject(typeof(ISisoSpatial), "SetPolygon")]
        public class when_setting_polygon_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.DefaultPolygon();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPolygon<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "InsertPolygon")]
        public class when_inserting_polygon_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.DefaultPolygon();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "DeleteGeoFor")]
        public class when_deleting_existing_polygon : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());

                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.DeleteGeoFor<SpatialGuidItem>(_item.StructureId);
                }
            };

            It should_have_no_geo_record_left =
                () => TestContext.DbHelper.should_have_been_deleted_from_spatial_table(_structureSchema, _item.StructureId);

            It should_now_return_empty_array_of_coordinates = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(new Coordinates[0]);
                }
            };

            private static SpatialGuidItem _item;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "DeleteGeoFor")]
        public class when_deleting_non_existing_polygon : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    _structureSchema = session.GetStructureSchema<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.DeleteGeoFor<SpatialGuidItem>(_item.StructureId);
                }
            };

            It should_have_no_geo_record_left =
                () => TestContext.DbHelper.should_have_been_deleted_from_spatial_table(_structureSchema, _item.StructureId);

            It should_now_return_empty_array_of_coordinates = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(new Coordinates[0]);
                }
            };

            private static SpatialGuidItem _item;
            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "SetPolygon")]
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

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygon<SpatialGuidItem>(_item.StructureId, orgCoordinates);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPolygon<SpatialGuidItem>(_item.StructureId, _newCoordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "UpdatePolygon")]
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

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPolygon<SpatialGuidItem>(_item.StructureId, orgCoordinates);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.UpdatePolygon<SpatialGuidItem>(_item.StructureId, _newCoordinates);
                }
            };

            It should_have_stored_a_new_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates[] _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_polygon_contains_point_that_is_within_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPoint<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.PointWithinDefaultPolygon);
                }
            };

            It should_be_contained =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_polygon_contains_point_that_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPoint<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.PointOutsideDefaultPolygon);
                }
            };

            It should_not_be_contained =
                () => _contains.ShouldBeFalse();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPointAfterExpand")]
        public class when_checking_if_a_expanded_polygon_contains_point_that_normally_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAfterExpand<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.PointOutsideDefaultPolygon, 4000d);
                }
            };

            It should_be_contained_after_expanding =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "SetPoint")]
        public class when_setting_point_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.PointWithinDefaultPolygon;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPoint<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            It should_have_inserted_one_record_with_the_point = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).Single().ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "InsertPoint")]
        public class when_inserting_point_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.PointWithinDefaultPolygon;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertPoint<SpatialGuidItem>(_item.StructureId, _coordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).Single().ShouldBeValueEqualTo(_coordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "SetPoint")]
        public class when_setting_point_and_one_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPoint<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Point1);
                }
                _newCoordinates = SpatialDataFactory.Point2;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetPoint<SpatialGuidItem>(_item.StructureId, _newCoordinates);
                }
            };

            It should_have_inserted_one_record_with_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).Single().ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "UpdatePoint")]
        public class when_updating_an_existing_point : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetPoint<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Point1);
                }
                _newCoordinates = SpatialDataFactory.Point2;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.UpdatePoint<SpatialGuidItem>(_item.StructureId, _newCoordinates);
                }
            };

            It should_have_stored_a_new_point = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).Single().ShouldBeValueEqualTo(_newCoordinates);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _newCoordinates;
        }

        [Subject(typeof(ISisoSpatial), "SetCircle")]
        public class when_setting_circle_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.Circle1;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetCircle<SpatialGuidItem>(_item.StructureId, _coordinates, 10d);
                }
            };

            It should_have_inserted_one_record_as_polygon_with_points_forming_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId);
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653063500926457d);
                    c[0].Longitude.ShouldEqual(-122.35790573151685d);
                    c[60].Latitude.ShouldEqual(47.652950145234975d);
                    c[60].Longitude.ShouldEqual(-122.35811079200715d);
                    c[128].Latitude.ShouldEqual(47.653063500926457d);
                    c[128].Longitude.ShouldEqual(-122.35790573151685d);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "InsertCircle")]
        public class when_inserting_circle_and_none_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                _coordinates = SpatialDataFactory.Circle1;
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertCircle<SpatialGuidItem>(_item.StructureId, _coordinates, 10d);
                }
            };

            It should_have_inserted_one_record_as_polygon_with_points_forming_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId);
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653063500926457d);
                    c[0].Longitude.ShouldEqual(-122.35790573151685d);
                    c[60].Latitude.ShouldEqual(47.652950145234975d);
                    c[60].Longitude.ShouldEqual(-122.35811079200715d);
                    c[128].Latitude.ShouldEqual(47.653063500926457d);
                    c[128].Longitude.ShouldEqual(-122.35790573151685d);
                }
            };

            private static SpatialGuidItem _item;
            private static Coordinates _coordinates;
        }

        [Subject(typeof(ISisoSpatial), "SetCircle")]
        public class when_setting_circle_and_one_exists_before : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 10d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.SetCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            It should_have_inserted_one_record_as_polygon_with_points_forming_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId);
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653031750472969d);
                    c[0].Longitude.ShouldEqual(-122.35795286578694d);
                    c[60].Latitude.ShouldEqual(47.652975072630916d);
                    c[60].Longitude.ShouldEqual(-122.35805539602994d);
                    c[128].Latitude.ShouldEqual(47.653031750472969d);
                    c[128].Longitude.ShouldEqual(-122.35795286578694d);
                }
            };

            private static SpatialGuidItem _item;
        }

        [Subject(typeof(ISisoSpatial), "UpdateCircle")]
        public class when_updating_an_existing_circle : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.SetCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.UpdateCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 10d);
                }
            };

            It should_have_stored_a_new_circle = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    var c = s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId);
                    c.Length.ShouldEqual(129);
                    c[0].Latitude.ShouldEqual(47.653063500926457d);
                    c[0].Longitude.ShouldEqual(-122.35790573151685d);
                    c[60].Latitude.ShouldEqual(47.652950145234975d);
                    c[60].Longitude.ShouldEqual(-122.35811079200715d);
                    c[128].Latitude.ShouldEqual(47.653063500926457d);
                    c[128].Longitude.ShouldEqual(-122.35790573151685d);
                }
            };

            private static SpatialGuidItem _item;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_circle_contains_point_that_is_within_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPoint<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1);
                }
            };

            It should_be_contained =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPoint")]
        public class when_checking_if_circle_contains_point_that_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPoint<SpatialGuidItem>(_item.StructureId, new Coordinates{Latitude = SpatialDataFactory.Circle1.Latitude + 0.05});
                }
            };

            It should_not_be_contained =
                () => _contains.ShouldBeFalse();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "ContainsPointAfterExpand")]
        public class when_checking_if_a_expanded_circle_contains_point_that_normally_is_outside_bounds : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                _contains = false;
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                    s.InsertCircle<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.Circle1, 5d);
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    _contains = s.ContainsPointAfterExpand<SpatialGuidItem>(_item.StructureId, new Coordinates
                    {
                        Latitude = SpatialDataFactory.Circle1.Latitude + 0.05, 
                        Longitude = SpatialDataFactory.Circle1.Longitude
                    }, 5600d);
                }
            };

            It should_be_contained =
                () => _contains.ShouldBeTrue();

            private static SpatialGuidItem _item;
            private static bool _contains;
        }

        [Subject(typeof(ISisoSpatial), "MakeValid")]
        public class when_makevalid_on_correctly_inserted_polygon : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.DefaultPolygon());
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.MakeValid<SpatialGuidItem>(_item.StructureId);
                }
            };

            It should_still_have_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeValueEqualTo(SpatialDataFactory.DefaultPolygon());
                }
            };

            private static SpatialGuidItem _item;
        }

        [Subject(typeof(ISisoSpatial), "InsertPolygon")]
        public class when_trying_to_insert_invalid_polygon : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                using (var session = TestContext.Database.BeginSession())
                {
                    _item = new SpatialGuidItem();
                    session.Insert(_item);

                    var s = session.Spatial();
                    s.EnableFor<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                CaughtException = Catch.Exception(() =>
                {
                    using (var session = TestContext.Database.BeginSession())
                    {
                        var s = session.Spatial();
                        s.InsertPolygon<SpatialGuidItem>(_item.StructureId, SpatialDataFactory.InvalidPolygonCauseOfMultiPolygon());
                    }
                });
            };

            It should_have_caused_an_exception = () =>
            {
                CaughtException.ShouldNotBeNull();
                CaughtException.ShouldBeOfType<SisoDbException>();
                CaughtException.Message.ShouldStartWith(ExceptionMessages.NotAValidPolygon.Inject("24409: Not valid because some portion of polygon ring (1) lies in the interior of a polygon."));
            };

            It should_not_have_inserted_the_polygon = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatial();
                    s.GetCoordinatesIn<SpatialGuidItem>(_item.StructureId).ShouldBeEmpty();
                }
            };

            private static SpatialGuidItem _item;
        }
    }
#endif
}
