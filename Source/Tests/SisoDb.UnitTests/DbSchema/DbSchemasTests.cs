using System;
using NUnit.Framework;
using SisoDb.Dac;
using SisoDb.DbSchema;
using SisoDb.NCore.Collections;

namespace SisoDb.UnitTests.DbSchema
{
    [TestFixture]
    public class DbSchemasTests : UnitTestBase
    {
        [Test]
        public void SysTables_value_of_Identities()
        {
            Assert.AreEqual("SisoDbIdentities", DbSchemas.SysTables.IdentitiesTable);
        }

        [Test]
        public void Suffixes_value_of_StructureTableNameSuffix()
        {
            Assert.AreEqual("Structure", DbSchemas.Suffixes.StructureTableNameSuffix);
        }

        [Test]
        public void Suffixes_value_of_UniquesTableNameSuffix()
        {
            Assert.AreEqual("Uniques", DbSchemas.Suffixes.UniquesTableNameSuffix);
        }

        [Test]
        public void Suffixes_value_of_IndexesTableNameSuffixes()
        {
            var expected = Enum.GetNames(typeof(IndexesTypes));

            Assert.AreEqual(expected, DbSchemas.Suffixes.IndexesTableNameSuffixes);
        }

        [Test]
        public void Suffixes_AllSuffixes_Should_contain_all_suffixes()
        {
            var expected = new[] { DbSchemas.Suffixes.StructureTableNameSuffix, DbSchemas.Suffixes.UniquesTableNameSuffix }.MergeWith(Enum.GetNames(typeof(IndexesTypes)));

            Assert.AreEqual(expected, DbSchemas.Suffixes.AllSuffixes);
        }

        [Test]
        public void Parameters_value_of_DbNameParamPrefix()
        {
            Assert.AreEqual("@dbName", DbSchemas.Parameters.DbNameParamPrefix);
        }

        [Test]
        public void Parameters_value_of_TableNameParamPrefix()
        {
            Assert.AreEqual("@tableName", DbSchemas.Parameters.TableNameParamPrefix);
        }

        [Test]
        public void Parameters_value_of_EntityNameParamPrefix()
        {
            Assert.AreEqual("@entityName", DbSchemas.Parameters.EntityNameParamPrefix);
        }
    }
}