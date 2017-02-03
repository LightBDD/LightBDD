using LightBDD.Framework.Formatting;
using NUnit.Framework;

namespace LightBDD.Framework.UnitTests.Formatting
{
	[TestFixture]
	public class DefaultNameFormatter_tests
	{
		[Test]
		[TestCase("Should_change_underscore_to_space", "Should change underscore to space")]
		[TestCase("Should_change_double_underscore_to__colon", "Should change double underscore to: colon")]
		[TestCase("It_s_Paul_s_cat", "It's Paul's cat")]
		public void It_should_format_text(string input, string expectedOutput)
		{
			Assert.That(new DefaultNameFormatter().FormatName(input), Is.EqualTo(expectedOutput));
		}
	}
}
