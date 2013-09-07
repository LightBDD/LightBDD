using NUnit.Framework;

namespace SimpleBDD.UnitTests
{
	[TestFixture]
	public class TextFormatterTests
	{
		[Test]
		[TestCase("Should_change_underscore_to_space", "Should change underscore to space")]
		[TestCase("Should_change_double_underscore_to__colon", "Should change double underscore to: colon")]
		[TestCase("It_s_Paul_s_cat", "It's Paul's cat")]
		public void Should_format_text(string input, string expectedOutput)
		{
			Assert.That(TextFormatter.Format(input), Is.EqualTo(expectedOutput));
		}
	}
}
