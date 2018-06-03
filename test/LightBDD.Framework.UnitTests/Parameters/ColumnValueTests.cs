using LightBDD.Framework.Formatting.Values;
using LightBDD.Framework.Parameters;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Parameters
{
    [TestFixture]
    public class ColumnValueTests
    {
        [Test]
        public void None_should_return_no_value()
        {
            Assert.IsNull(ColumnValue.None.Value);
            Assert.IsFalse(ColumnValue.None.HasValue);
            Assert.AreEqual("<none>", ColumnValue.None.ToString());
            Assert.AreEqual("<none>", ColumnValue.None.Format(ValueFormattingServices.Current));
        }

        [Test]
        [TestCase(null, "<null>")]
        [TestCase(321, "321")]
        public void From_should_return_initialized_column_value(object value, string expectedFormat)
        {
            var columnValue = ColumnValue.From(value);
            Assert.AreEqual(value, columnValue.Value);
            Assert.IsTrue(columnValue.HasValue);
            Assert.AreEqual(expectedFormat, columnValue.ToString());
            Assert.AreEqual(expectedFormat, columnValue.Format(ValueFormattingServices.Current));
        }
    }
}
