using System;
using System.Linq;
using NUnit.Framework;
using SisoDb.Structures.Schemas;
using SisoDb.Structures.Schemas.Builders;

namespace SisoDb.UnitTests.Structures.Schemas
{
    [TestFixture]
    public class StructureSchemasTests : UnitTestBase
    {
        [Test]
        public void GetSchema_InvokedUsingGenerics_WillGenerateSchema()
        {
            var schemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());

            var schema = schemas.GetSchema<X>();

            Assert.IsNotNull(schema);
        }

        [Test]
        public void GetSchema_InvokedUsingType_WillGenerateSchema()
        {
            var schemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());

            var schema = schemas.GetSchema(typeof(X));

            Assert.IsNotNull(schema);
        }

        [Test]
        public void GetSchemas_WhenEmpty_YieldsEmptyStream()
        {
            var schemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());

            var numOfSchemas = schemas.GetSchemas().Count();

            Assert.AreEqual(0, numOfSchemas);
        }

        [Test]
        public void GetSchemas_WhenContainingASchema_YieldsOneSchema()
        {
            var schemas = new StructureSchemas(new StructureTypeFactory(), new AutoStructureSchemaBuilder());
            var schema = schemas.GetSchema<X>();

            var fetchedSchema = schemas.GetSchemas().First();

            Assert.AreEqual(schema, fetchedSchema);
        }

		[Test]
		public void GetSchema_WhenCustomConfigurationExists_ReturnsSchemaWithCorrectIndexAccessor()
		{
            var structureTypeFactory = new StructureTypeFactory();
            var schemaBuilder = new AutoStructureSchemaBuilder();
            var schemas = new StructureSchemas(structureTypeFactory, schemaBuilder);
			schemas.StructureTypeFactory.Configurations.Configure<FooCustomer>(cfg => cfg.OnlyIndexThis("CustomerNo"));

			var schema = schemas.GetSchema<FooCustomer>();

			Assert.AreEqual("CustomerNo", schema.IndexAccessors.Single().Path);
		}

		[Test]
		public void GetSchema_WhenCustomConfigurationExists_ViaLambdas_ReturnsSchemaWithCorrectIndexAccessor()
		{
            var structureTypeFactory = new StructureTypeFactory();
            var schemaBuilder = new AutoStructureSchemaBuilder();
            var schemas = new StructureSchemas(structureTypeFactory, schemaBuilder);
			schemas.StructureTypeFactory.Configurations.Configure<FooCustomer>(cfg => cfg.OnlyIndexThis(c => c.CustomerNo));

			var schema = schemas.GetSchema<FooCustomer>();

			Assert.AreEqual("CustomerNo", schema.IndexAccessors.Single().Path);
		}

		[Test]
		public void GetSchema_WhenCustomConfigurationExists_ViaNonGenericConfig_ReturnsSchemaWithCorrectIndexAccessor()
		{
            var structureTypeFactory = new StructureTypeFactory();
            var schemaBuilder = new AutoStructureSchemaBuilder();
            var schemas = new StructureSchemas(structureTypeFactory, schemaBuilder);
            schemas.StructureTypeFactory.Configurations.Configure(typeof(FooCustomer), cfg => cfg.OnlyIndexThis("CustomerNo"));

			var schema = schemas.GetSchema<FooCustomer>();

			Assert.AreEqual("CustomerNo", schema.IndexAccessors.Single().Path);
		}

		private class FooCustomer
		{
			public Guid StructureId { get; set; }
			public int CustomerNo { get; set; }
			public string Firstname { get; set; }
			public string Lastname { get; set; }

			public override string ToString()
			{
				return string.Format("StructureId: {0}, CustomerNo: {1}, Firstname: {2}, Lastname: {3}", StructureId, CustomerNo, Firstname, Lastname);
			}
		}

        private class X
        {
            public Guid StructureId { get; set; }

            public int IntOfX { get; set; }
        }
    }
}