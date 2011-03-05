using System;
using NUnit.Framework;
using SisoDb.Providers.SqlProvider;
using SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.TestModel;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Inserts
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
            var idForRoot = new Guid("FC424BFD-1C50-4615-8492-BC2EF53ACAB3");
            var idForNested = new Guid("6244FB01-EECA-4EBC-806E-61A22D74D454");
            var rootStructure = new MyFirstStructure
                                {
                                    Id = idForRoot,
                                    Value = "My first structure.",
                                    NestedStructure = new MySecondStructure
                                                      {
                                                          Id = idForNested,
                                                          Value = "My second structure."
                                                      }
                                };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(rootStructure);
                uow.Commit();

                var json = uow.GetByIdAsJson<MyFirstStructure>(idForRoot);
                Assert.AreEqual("{\"Id\":\"fc424bfd1c5046158492bc2ef53acab3\",\"Value\":\"My first structure.\"}", json);
            }
        }

        [Test]
        public void Insert_WhenNestedStructureHasValues_NestedStructureIsNotIndexed()
        {
            var idForRoot = new Guid("FC424BFD-1C50-4615-8492-BC2EF53ACAB3");
            var idForNested = new Guid("6244FB01-EECA-4EBC-806E-61A22D74D454");
            var rootStructure = new MyFirstStructure
            {
                Id = idForRoot,
                Value = "My first structure.",
                NestedStructure = new MySecondStructure
                {
                    Id = idForNested,
                    Value = "My second structure."
                }
            };

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(rootStructure);
                uow.Commit();

                var schema = Database.StructureSchemas.GetSchema<MyFirstStructure>();
                var valueColumnExistsForNestedInFirstStructure = DbHelper.ColumnsExist(
                    schema.GetIndexesTableName(),
                    SisoDbEnvironment.MemberNameGenerator.Generate("NestedStructure.Value"));

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
                Id = idForRoot,
                Value = "My first structure.",
                NestedStructure = new MySecondStructure
                {
                    Id = idForNested,
                    Value = "My second structure."
                }
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
            public Guid Id { get; set; }

            public string Value { get; set; }

            public MySecondStructure NestedStructure { get; set; }
        }

        private class MySecondStructure
        {
            public Guid Id { get; set; }

            public string Value { get; set; }
        }
    }

    [TestFixture]
    public class SqlUnitOfWorkInsertTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<Root>();
            DropStructureSet<TheChild>();
        }

        [Test]
        public void Insert_EntityGraphWithMixedValues_ValuesAreStoredInDb()
        {
            var entityGraph = CreateInitializedEntityGraph();

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(entityGraph);
                unitOfWork.Commit();
            }

            Root refetched = null;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.GetById<Root>(entityGraph.Id);
            }

            CustomAssert.AreValueEqual(entityGraph, refetched);
        }

        [Test]
        public void Insert_InheritedEntityWithMixedValues_ValuesAreStoredInDb()
        {
            var entityGraph = CreateInitializedInheritedEntity();

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(entityGraph);
                unitOfWork.Commit();
            }

            TheChild refetched = null;
            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                refetched = unitOfWork.GetById<TheChild>(entityGraph.Id);
            }

            CustomAssert.AreValueEqual(entityGraph, refetched);
        }

        private static Root CreateInitializedEntityGraph()
        {
            return new Root
                   {
                       RootInt1 = 1,
                       RootString1 = "Root string 1",
                       Nested = new Nested
                                {
                                    NestedInt1 = 2,
                                    NestedString1 = "Nested string 1"
                                }
                   };
        }

        private static TheChild CreateInitializedInheritedEntity()
        {
            return new TheChild
                   {
                       BaseInt1 = 1,
                       BaseString1 = "Base string 1",
                       ChildInt1 = 2,
                       ChildString1 = "Child string 1"
                   };
        }
    }
}
