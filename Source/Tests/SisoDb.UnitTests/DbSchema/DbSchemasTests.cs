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
            Assert.AreEqual("SisoDbIdentities", DbSchemaInfo.SysTables.IdentitiesTable);
        }

        [Test]
        public void Suffixes_value_of_StructureTableNameSuffix()
        {
            Assert.AreEqual("Structure", DbSchemaInfo.Suffixes.StructureTableNameSuffix);
        }

        [Test]
        public void Suffixes_value_of_UniquesTableNameSuffix()
        {
            Assert.AreEqual("Uniques", DbSchemaInfo.Suffixes.UniquesTableNameSuffix);
        }

        [Test]
        public void Suffixes_value_of_IndexesTableNameSuffixes()
        {
            var expected = Enum.GetNames(typeof(IndexesTypes));

            Assert.AreEqual(expected, DbSchemaInfo.Suffixes.IndexesTableNameSuffixes);
        }

        [Test]
        public void Suffixes_AllSuffixes_Should_contain_all_suffixes()
        {
            var expected = new[] { DbSchemaInfo.Suffixes.StructureTableNameSuffix, DbSchemaInfo.Suffixes.UniquesTableNameSuffix }.MergeWith(Enum.GetNames(typeof(IndexesTypes)));

            Assert.AreEqual(expected, DbSchemaInfo.Suffixes.AllSuffixes);
        }

        [Test]
        public void Parameters_value_of_DbNameParamPrefix()
        {
            Assert.AreEqual("@dbName", DbSchemaInfo.Parameters.DbNameParamPrefix);
        }

        [Test]
        public void Parameters_value_of_TableNameParamPrefix()
        {
            Assert.AreEqual("@tableName", DbSchemaInfo.Parameters.TableNameParamPrefix);
        }

        [Test]
        public void Parameters_value_of_EntityNameParamPrefix()
        {
            Assert.AreEqual("@entityName", DbSchemaInfo.Parameters.EntityNameParamPrefix);
        }
    }
}