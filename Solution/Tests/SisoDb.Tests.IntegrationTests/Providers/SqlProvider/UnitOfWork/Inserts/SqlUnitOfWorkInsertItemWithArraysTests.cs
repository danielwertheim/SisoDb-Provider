using System;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.SqlProvider.UnitOfWork.Inserts
{
    [TestFixture]
    public class SqlUnitOfWorkInsertItemWithArraysTests : SqlIntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<Level1>();
        }

        [Test]
        public void Insert_ItemWithSimpleArrayValues_ValuesAreStoredInDb()
        {
            var item = new Level1();
            item.Integers = new[] { 1, 1, 2 };
            item.Strings = new[] { "A", "A", "B" };
            item.Dates = new[] { new DateTime(2010, 1, 2, 3, 4, 5, 6), new DateTime(2010, 1, 2, 3, 4, 5, 6), new DateTime(2010, 1, 2, 3, 4, 5, 7) };
            item.Bytes = BitConverter.GetBytes(42);
            item.Level2Items = new[]
            {
                new Level2 { StringValue= "StringA" }, 
                new Level2 { StringValue = "StringA" }, 
                new Level2
                { 
                    StringValue = "StringB", 
                    IntegerValue = 1,
                    Strings = new []{"Strings1", "Strings2"}, 
                    Integers = new []{0,1,2,4,8},
                    Level3Items = new []
                    {
                        new Level3 { Integers = new []{1,3} },
                        new Level3 { Integers = new []{2,4} }
                    }
                }
            };

            using (var unitOfWork = Database.CreateUnitOfWork())
            {
                unitOfWork.Insert(item);
                unitOfWork.Commit();
            }

            var memberNameGenerator = SisoEnvironment.Resources.ResolveMemberNameGenerator();
            var level1IntegersHash = memberNameGenerator.Generate("Integers");
            var level1StringsHash = memberNameGenerator.Generate("Strings");
            var level1DatesHash = memberNameGenerator.Generate("Dates");
            var level1BytesHash = memberNameGenerator.Generate("Bytes");
            var level2StringValueMemberHash = memberNameGenerator.Generate("Level2Items.StringValue");
            var level2IntegerValueMemberHash = memberNameGenerator.Generate("Level2Items.IntegerValue");
            var level2StringsMemberHash = memberNameGenerator.Generate("Level2Items.Strings");
            var level2IntegersMemberHash = memberNameGenerator.Generate("Level2Items.Integers");
            var level3IntegersMemberHash = memberNameGenerator.Generate("Level2Items.Level3Items.Integers");
            var indexesTable = DbHelper.GetTableBySql("select * from dbo.Level1Indexes");
            Assert.AreEqual(1, indexesTable.Rows.Count);
            Assert.IsFalse(indexesTable.Columns.Contains(level1BytesHash));
            Assert.AreEqual("<$1$><$2$>", indexesTable.Rows[0][level1IntegersHash]);
            Assert.AreEqual("<$A$><$B$>", indexesTable.Rows[0][level1StringsHash]);
            Assert.AreEqual("<$2010-01-02 03:04:05.006$><$2010-01-02 03:04:05.007$>", indexesTable.Rows[0][level1DatesHash]);
            Assert.AreEqual("<$StringA$><$StringB$>", indexesTable.Rows[0][level2StringValueMemberHash]);
            Assert.AreEqual("<$0$><$1$>", indexesTable.Rows[0][level2IntegerValueMemberHash]);
            Assert.AreEqual("<$$><$Strings1$><$Strings2$>", indexesTable.Rows[0][level2StringsMemberHash]);
            Assert.AreEqual("<$$><$0$><$1$><$2$><$4$><$8$>", indexesTable.Rows[0][level2IntegersMemberHash]);
            Assert.AreEqual("<$$><$1$><$3$><$2$><$4$>", indexesTable.Rows[0][level3IntegersMemberHash]);
        }

        public class Level1
        {
            public Guid SisoId { get; set; }

            public Level2[] Level2Items { get; set; }

            public string[] Strings { get; set; }

            public DateTime[] Dates { get; set; }

            public int[] Integers { get; set; }

            public byte[] Bytes { get; set; }
        }

        public class Level2
        {
            public string StringValue { get; set; }
            public string[] Strings { get; set; }

            public int IntegerValue { get; set; }
            public int[] Integers { get; set; }

            public Level3[] Level3Items { get; set; }
        }

        public class Level3
        {
            public int[] Integers { get; set; }
        }
    }
}