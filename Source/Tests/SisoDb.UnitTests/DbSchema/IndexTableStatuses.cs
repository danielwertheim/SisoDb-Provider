using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class IndexTableStatuses : UnitTestBase
    {
        [Test]
        public void AllExists_WhenAllExists_ReturnsTrue()
        {
            var statuses = CreateStatusesWithAllExisting();

            Assert.IsTrue(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenIntegersDoesNotExist_ReturnsFalse()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.IntegersTableExists = false;

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenFractalsDoesNotExist_ReturnsFalse()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.FractalsTableExists = false;

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenBooleansDoesNotExist_ReturnsFalse()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.BooleansTableExists = false;

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenGuidsDoesNotExist_ReturnsFalse()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.GuidsTableExists = false;

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenDatesDoesNotExist_ReturnsFalse()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.DatesTableExists = false;

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenStringsDoesNotExist_ReturnsFalse()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.StringsTableExists = false;

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenTextsDoesNotExist_ReturnsFalse()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.TextsTableExists = false;

            Assert.IsFalse(statuses.AllExists);
        }

        private static IndexesTableStatuses CreateStatusesWithAllExisting()
        {
            var statuses = new IndexesTableStatuses(new IndexesTableNames("MyStructure"));
            statuses.IntegersTableExists = true;
            statuses.FractalsTableExists = true;
            statuses.BooleansTableExists = true;
            statuses.GuidsTableExists = true;
            statuses.DatesTableExists = true;
            statuses.StringsTableExists = true;
            statuses.TextsTableExists = true;

            return statuses;
        }
    }
}