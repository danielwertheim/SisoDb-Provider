using NUnit.Framework;
using PineCone.Structures;
using SisoDb.Structures;

namespace SisoDb.UnitTests.Structures
{
    [TestFixture]
    public class DataTypeCodeExtensions : UnitTestBase
    {
        [TestCase(DataTypeCode.Bool)]
        [TestCase(DataTypeCode.DateTime)]
        [TestCase(DataTypeCode.FractalNumber)]
        [TestCase(DataTypeCode.Guid)]
        [TestCase(DataTypeCode.IntegerNumber)]
        public void IsValueType_WhenValueType_ReturnsTrue(DataTypeCode dataTypeCode)
        {
            Assert.IsTrue(dataTypeCode.IsValueType());
        }

        [TestCase(DataTypeCode.Enum)]
        [TestCase(DataTypeCode.String)]
        [TestCase(DataTypeCode.Text)]
        public void IsValueType_WhenNonValueType_ReturnsFalse(DataTypeCode dataTypeCode)
        {
            Assert.IsFalse(dataTypeCode.IsValueType());
        }
    }
}