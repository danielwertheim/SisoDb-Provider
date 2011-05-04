using System;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Providers.Sql2008Provider;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008Provider.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkInsertWithNestedStructuresTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<MyFirstStructure>();
            DropStructureSet<MySecondStructure>();
        }

        [Test]
        public void Insert_WhenNestedStructureHasValues_NestedStructureIsNotStoredInJson()
        {
            var rootStructure = new MyFirstStructure
                                {
                                    Value = "My first structure.",
                                    NestedStructure = new MySecondStructure
                                                      {
                                                          Value = "My second structure."
                                                      },
                                    NestedObject =  new MyValueObject{Value = "My value object."}
                                };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(rootStructure);
                uow.Commit();

                var json = uow.GetByIdAsJson<MyFirstStructure>(rootStructure.SisoId);
                Assert.AreEqual("{\"SisoId\":\"" + rootStructure.SisoId.ToString("N") + "\",\"Value\":\"My first structure.\",\"NestedObject\":{\"Value\":\"My value object.\"}}", json);
            }
        }

        [Test]
        public void Insert_WhenNestedStructureHasValues_NestedStructureIsNotIndexed()
        {
            var idForRoot = new Guid("FC424BFD-1C50-4615-8492-BC2EF53ACAB3");
            var idForNested = new Guid("6244FB01-EECA-4EBC-806E-61A22D74D454");
            var rootStructure = new MyFirstStructure
                                {
                                    SisoId = idForRoot,
                                    Value = "My first structure.",
                                    NestedStructure = new MySecondStructure
                                                      {
                                                          SisoId = idForNested,
                                                          Value = "My second structure."
                                                      },
                                    NestedObject =  new MyValueObject{Value = "My value object."}
                                };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(rootStructure);
                uow.Commit();

                var schema = Database.StructureSchemas.GetSchema(TypeFor<MyFirstStructure>.Type);
                var valueColumnExistsForNestedInFirstStructure = DbHelper.ColumnsExist(
                    schema.GetIndexesTableName(),
                    SisoEnvironment.Resources.ResolveMemberNameGenerator().Generate("NestedStructure.Value"));

                Assert.IsFalse(valueColumnExistsForNestedInFirstStructure);
            }
        }

        [Test]
        public void Insert_WhenNestedStructureHasValues_NestedStructureIsNotStoredOnItsOwn()
        {
            var idForRoot = new Guid("FC424BFD-1C50-4615-8492-BC2EF53ACAB3");
            var idForNested = new Guid("6244FB01-EECA-4EBC-806E-61A22D74D454");
            var rootStructure = new MyFirstStructure
                                {
                                    SisoId = idForRoot,
                                    Value = "My first structure.",
                                    NestedStructure = new MySecondStructure
                                                      {
                                                          SisoId = idForNested,
                                                          Value = "My second structure."
                                                      },
                                    NestedObject = new MyValueObject { Value = "My value object." }
                                };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(rootStructure);
                uow.Commit();

                var secondStructures = uow.GetAll<MySecondStructure>();
                CollectionAssert.IsEmpty(secondStructures);
            }
        }

        private class MyFirstStructure
        {
            public Guid SisoId { get; set; }

            public string Value { get; set; }

            public MySecondStructure NestedStructure { get; set; }

            public MyValueObject NestedObject { get; set; }
        }

        private class MySecondStructure
        {
            public Guid SisoId { get; set; }

            public string Value { get; set; }
        }

        private class MyValueObject
        {
            public string Value { get; set; }
        }
    }
}