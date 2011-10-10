using System;
using System.Linq;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Sql2008.UnitOfWork.Inserts
{
    [TestFixture]
    public class Sql2008UnitOfWorkJsonInsertTests : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<MyClass>();
            DropStructureSet<MyGuidClass>();
        }

        [Test]
        public void Insert_WhenJsonHasValuesForAllMembers_AllMembersAreMapped()
        {
            const string json = "{\"String1\":\"The string 1\", \"Int1\":42, \"Decimal1\": 12.34}";

            using(var uow = Database.CreateUnitOfWork())
            {
                uow.InsertJson<MyClass>(json);
                uow.Commit();

                var item = uow.GetAll<MyClass>().Single();
                Assert.AreEqual("The string 1", item.String1);
                Assert.AreEqual(42, item.Int1);
                Assert.AreEqual(12.34M, item.Decimal1);
            }
        }

        [Test]
        public void Insert_WhenMembernameDiffersInCasing_MemberIsNotMapped()
        {
            const string json = "{\"string1\":\"The string 1\"}";

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertJson<MyClass>(json);
                uow.Commit();

                var item = uow.GetAll<MyClass>().Single();
                Assert.IsNull(item.String1);
            }
        }

        [Test]
        public void Insert_WhenIdentityMemberHasValue_ValueIsNotInjectedToMember()
        {
            var structureId = 123;
            var json = string.Format("{{\"StructureId\":{0}}}", structureId);

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertJson<MyClass>(json);
                uow.Commit();

                var item = uow.GetAll<MyClass>().Single();
                Assert.AreNotEqual(structureId, item.StructureId);
            }
        }

        [Test]
        public void Insert_WhenGuidIdMemberHasValue_ValueIsNotInjectedToMember()
        {
            var structureId = SequentialGuid.NewSqlCompatibleGuid();
            var json = string.Format("{{\"StructureId\":{0}}}", structureId);

            using (var uow = Database.CreateUnitOfWork())
            {
                uow.InsertJson<MyGuidClass>(json);
                uow.Commit();

                var item = uow.GetAll<MyGuidClass>().Single();
                Assert.AreNotEqual(structureId, item.StructureId);
            }
        }

        private class MyClass
        {
            public int StructureId { get; set; }

            public string String1 { get; set; }

            public int Int1 { get; set; }

            public decimal Decimal1 { get; set; }
        }

        private class MyGuidClass
        {
            public Guid StructureId { get; set; }

            public string String1 { get; set; }

            public int Int1 { get; set; }

            public decimal Decimal1 { get; set; }
        }
    }
}