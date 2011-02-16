using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SisoDb.Dynamic;

namespace SisoDb.Tests.UnitTests.Dynamic
{
    //TODO: Split tests...
    [TestFixture]
    public class DynamicStructureTests
    {
        [Test]
        public void AccessPropertes_AfterInjectedKeyValues_CanRetrieveAsCorrectTypes()
        {
            var kv = new Dictionary<string, object>
            {
                {"String1", "String1 value"}, {"Int1", 42}, {"DateTime1", new DateTime(2011, 01, 01, 23, 59, 00)}
            };

            dynamic ds = new DynamicStructure(kv);

            Assert.AreEqual(kv["String1"], ds.String1);
            Assert.AreEqual(kv["Int1"], ds.Int1);
            Assert.AreEqual(kv["DateTime1"], ds.DateTime1);
        }

        [Test]
        public void GetDynamicMemberNames_AfterDynamicAssignment_AllPropertiesAreRepresented()
        {
            var kv = new Dictionary<string, object>
            {
                {"String1", "String1 value"}, {"Int1", 42}, {"DateTime1", new DateTime(2011, 01, 01, 23, 59, 00)}
            };

            dynamic ds = new DynamicStructure();
            ds.String1 = kv["String1"];
            ds.Int1 = kv["Int1"];
            ds.DateTime1 = kv["DateTime1"];

            var memberNames = ((DynamicStructure) ds).GetDynamicMemberNames();
            CollectionAssert.AreEquivalent(kv.Keys, memberNames);
        }

        [Test]
        public void TypeDescriptor_AfterReassignmentOfMemberWithNewType_NewTypeIsStored()
        {
            dynamic ds = new DynamicStructure();
            ds.Int1 = 42;
            ds.Int1 = "Test";

            var ds2 = (DynamicStructure)ds;
            var member = ds2.TypeDescriptor.Single();
            
            Assert.AreEqual("Int1", member.Name);
            Assert.AreEqual(typeof(string), member.Type);
        }

        [Test]
        public void TypeDescriptor_AfterDynamicAssignment_NameAndTypeMatchesAssignments()
        {
            var kv = new Dictionary<string, object>
            {
                {"String1", "String1 value"}, {"Int1", 42}, {"DateTime1", new DateTime(2011, 01, 01, 23, 59, 00)}
            };

            dynamic ds = new DynamicStructure();
            ds.String1 = kv["String1"];
            ds.Int1 = kv["Int1"];
            ds.DateTime1 = kv["DateTime1"];

            var descriptor = ((DynamicStructure) ds).TypeDescriptor;
            var members = descriptor.OrderBy(m => m.Name).ToList();
            Assert.AreEqual("DateTime1", members[0].Name);
            Assert.AreEqual(typeof(DateTime), members[0].Type);
            Assert.AreEqual("Int1", members[1].Name);
            Assert.AreEqual(typeof(int), members[1].Type);
            Assert.AreEqual("String1", members[2].Name);
            Assert.AreEqual(typeof(string), members[2].Type);
        }

        [Test]
        public void TypeDescriptor_AfterInjectedKeyValues_NameAndTypeMatchesAssignments()
        {
            var kv = new Dictionary<string, object>
            {
                {"String1", "String1 value"}, {"Int1", 42}, {"DateTime1", new DateTime(2011, 01, 01, 23, 59, 00)}
            };

            var ds = new DynamicStructure(kv);

            var members = ds.TypeDescriptor.OrderBy(m => m.Name).ToList();
            Assert.AreEqual("DateTime1", members[0].Name);
            Assert.AreEqual(typeof(DateTime), members[0].Type);
            Assert.AreEqual("Int1", members[1].Name);
            Assert.AreEqual(typeof(int), members[1].Type);
            Assert.AreEqual("String1", members[2].Name);
            Assert.AreEqual(typeof(string), members[2].Type);
        }

        [Test]
        public void TryGet_AfterDynamicAssignment_ValuesCanBeRetrieved()
        {
            var kv = new Dictionary<string, object>
            {
                {"String1", "String1 value"}, {"Int1", 42}, {"DateTime1", new DateTime(2011, 01, 01, 23, 59, 00)}
            };

            dynamic ds = new DynamicStructure();
            ds.String1 = kv["String1"];
            ds.Int1 = kv["Int1"];
            ds.DateTime1 = kv["DateTime1"];

            Assert.AreEqual(kv["String1"], ds.String1);
            Assert.AreEqual(kv["Int1"], ds.Int1);
            Assert.AreEqual(kv["DateTime1"], ds.DateTime1);
        }
    }
}