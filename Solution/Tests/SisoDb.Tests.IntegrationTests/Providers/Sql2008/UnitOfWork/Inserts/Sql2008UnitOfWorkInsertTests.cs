using NUnit.Framework;
using SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.TestModel;
using SisoDb.TestUtils;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.UnitOfWork.Inserts
{
    [TestFixture]
    public class Sql2008UnitOfWorkInsertTests : Sql2008IntegrationTestBase
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
                refetched = unitOfWork.GetById<Root>(entityGraph.SisoId);
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
                refetched = unitOfWork.GetById<TheChild>(entityGraph.SisoId);
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
