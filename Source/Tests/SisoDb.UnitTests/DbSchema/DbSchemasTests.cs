using System;
using NCore.Collections;
using NUnit.Framework;
using SisoDb.DbSchema;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class DbSchemasTests : UnitTestBase
    {
        [Test]
        public void SysTables_Identities_ShouldBe_SisoDbIdentities()
        {
            Assert.AreEqual("SisoDbIdentities", DbSchemas.SysTables.IdentitiesTable);
        }

        [Test]
        public void Suffixes_StructureTableNameSuffix_ShouldBe_Structure()
        {
            Assert.AreEqual("Structure", DbSchemas.Suffixes.StructureTableNameSuffix);
        }

        [Test]
        public void Suffixes_UniquesTableNameSuffix_ShouldBe_Structure()
        {
            Assert.AreEqual("Uniques", DbSchemas.Suffixes.UniquesTableNameSuffix);
        }

        [Test]
        public void Suffixes_IndexesTableNameSuffixes_ShouldBe_Structure()
        {
            var expected = Enum.GetNames(typeof(IndexesTypes));

            Assert.AreEqual(expected, DbSchemas.Suffixes.IndexesTableNameSuffixes);
        }

        [Test]
        public void Suffixes_AllSuffixes_ShouldBe_Structure()
        {
            var expected = new[] { DbSchemas.Suffixes.StructureTableNameSuffix, DbSchemas.Suffixes.UniquesTableNameSuffix }.MergeWith(Enum.GetNames(typeof(IndexesTypes)));

            Assert.AreEqual(expected, DbSchemas.Suffixes.AllSuffixes);
        }
    }
}