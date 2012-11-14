using Machine.Specifications;
using SisoDb.Spatial;
using SisoDb.Specifications.Model;
using SisoDb.Structures.Schemas;
using SisoDb.Testing;

namespace SisoDb.Specifications.Spatial
{
#if Sql2008Provider || Sql2012Provider
    class UpsertAndDrop
    {
        [Subject(typeof(ISisoSpatial), "EnableFor")]
        public class when_upserting_guid_item_for_the_first_time : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<SpatialGuidItem>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SpatialGuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                }
            };

            It should_have_created_the_spatial_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetSpatialTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "EnableFor")]
        public class when_upserting_identity_item_for_the_first_time : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<SpatialIdentityItem>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SpatialIdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.EnableFor<SpatialIdentityItem>();
                }
            };

            It should_have_created_the_spatial_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetSpatialTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "EnableFor")]
        public class when_upserting_big_identity_item_for_the_first_time : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<SpatialBigIdentityItem>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SpatialBigIdentityItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.EnableFor<SpatialBigIdentityItem>();
                }
            };

            It should_have_created_the_spatial_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetSpatialTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "EnableFor")]
        public class when_upserting_string_item_for_the_first_time : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<SpatialStringItem>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SpatialStringItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.EnableFor<SpatialStringItem>();
                }
            };

            It should_have_created_the_spatial_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetSpatialTableName()).ShouldBeTrue();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "EnableFor")]
        public class when_dropping_without_any_existing : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<SpatialGuidItem>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SpatialGuidItem>();
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.RemoveFor<SpatialGuidItem>();
                }
            };

            It should_have_non_existing_spatial_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetSpatialTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema;
        }

        [Subject(typeof(ISisoSpatial), "EnableFor")]
        public class when_dropping_existing : SpecificationBase
        {
            Establish context = () =>
            {
                TestContext = TestContextFactory.Create();
                TestContext.Database.UpsertStructureSet<SpatialGuidItem>();
                _structureSchema = TestContext.Database.StructureSchemas.GetSchema<SpatialGuidItem>();
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.EnableFor<SpatialGuidItem>();
                }
            };

            Because of = () =>
            {
                using (var session = TestContext.Database.BeginSession())
                {
                    var s = session.Spatials();
                    s.RemoveFor<SpatialGuidItem>();
                }
            };

            It should_have_non_existing_spatial_table =
                () => TestContext.DbHelper.TableExists(_structureSchema.GetSpatialTableName()).ShouldBeFalse();

            private static IStructureSchema _structureSchema;
        }
    }
#endif
}