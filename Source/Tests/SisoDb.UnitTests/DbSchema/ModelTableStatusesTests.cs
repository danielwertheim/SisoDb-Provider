using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class ModelTableStatusesTests : UnitTestBase
    {
        [Test]
        public void AllExists_WhenAllExists_ReturnsTrue()
        {
            var statuses = CreateStatuses();

            Assert.IsTrue(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenStructureDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(stringsTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenUnqiueDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(uniquesTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenIntegersDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(integersTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenFractalsDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(fractalsTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenBooleansDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(booleansTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenGuidsDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(guidsTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenDatesDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(datesTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenStringsDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(stringsTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        [Test]
        public void AllExists_WhenTextsDoesNotExist_ReturnsFalse()
        {
            var statuses = CreateStatuses(textsTableExists: false);

            Assert.IsFalse(statuses.AllExists);
        }

        private static ModelTableStatuses CreateStatuses(
            bool structureTableExists = true,
            bool uniquesTableExists = true,
            bool integersTableExists = true,
            bool fractalsTableExists = true,
            bool datesTableExists = true,
            bool booleansTableExists = true,
            bool guidsTableExists = true,
            bool stringsTableExists = true,
            bool textsTableExists = true)
        {
            var indexesStatuses = new IndexesTableStatuses(
                integersTableExists,
                fractalsTableExists,
                datesTableExists,
                booleansTableExists,
                guidsTableExists,
                stringsTableExists,
                textsTableExists);

            return new ModelTableStatuses(structureTableExists, uniquesTableExists, indexesStatuses);
        }
    }
}