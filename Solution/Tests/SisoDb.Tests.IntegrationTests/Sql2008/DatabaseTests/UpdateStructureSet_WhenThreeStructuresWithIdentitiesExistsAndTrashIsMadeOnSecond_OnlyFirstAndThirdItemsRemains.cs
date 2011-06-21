using System;
using NUnit.Framework;
using SisoDb.Core;
using SisoDb.Tests.IntegrationTests.Sql2008.StructureSetUpdaterTests.ModelOld;

namespace SisoDb.Tests.IntegrationTests.Sql2008.DatabaseTests
{
    [TestFixture]
    public class UpdateStructureSet_WhenThreeStructuresWithIdentitiesExistsAndTrashIsMadeOnSecond_OnlyFirstAndThirdItemsRemains 
        : Sql2008TembDbIntegrationTestBase
    {
        protected override string TempDbName
        {
            get { return "CD4F1DF1797C4BC5A5BA7170B3D02581"; }
        }

        [Test]
        public void Test()
        {
            Database.EnsureNewDatabase();

            var orgItem1 = new ItemForPropChange { String1 = "A" };
            var orgItem2 = new ItemForPropChange { String1 = "B" };
            var orgItem3 = new ItemForPropChange { String1 = "C" };
            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertMany(new[] { orgItem1, orgItem2, orgItem3 });
                uow.Commit();
            }
            Database.StructureSchemas.RemoveSchema(TypeFor<ItemForPropChange>.Type);

            var id1 = orgItem1.SisoId;
            var id2 = orgItem2.SisoId;
            var id3 = orgItem3.SisoId;
            Database.UpdateStructureSet<ItemForPropChange, StructureSetUpdaterTests.ModelNew.ItemForPropChange>(
                (oldItem, newItem) =>
                {
                    newItem.NewString1 = oldItem.String1;
                    return oldItem.SisoId.Equals(id2)
                               ? StructureSetUpdaterStatuses.Trash
                               : StructureSetUpdaterStatuses.Keep;
                });

            using (var uow = Database.CreateUnitOfWork())
            {
                var newItem1 = uow.GetById<StructureSetUpdaterTests.ModelNew.ItemForPropChange>(id1);
                Assert.IsNotNull(newItem1);
                Assert.AreEqual("A", newItem1.NewString1);

                var newItem2 = uow.GetById<StructureSetUpdaterTests.ModelNew.ItemForPropChange>(id2);
                Assert.IsNull(newItem2);

                var newItem3 = uow.GetById<StructureSetUpdaterTests.ModelNew.ItemForPropChange>(id3);
                Assert.IsNotNull(newItem3);
                Assert.AreEqual("C", newItem3.NewString1);
            }
        }

        private class ItemForUpsertStructureSet
        {
            public Guid SisoId { get; set; }

            public string Temp
            {
                get { return "Some text to get rid of exception of no indexable members."; }
            }
        }
    }
}