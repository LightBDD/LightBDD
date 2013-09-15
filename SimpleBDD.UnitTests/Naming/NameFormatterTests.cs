using NUnit.Framework;
using SimpleBDD.Naming;

namespace SimpleBDD.UnitTests.Naming
{
	[TestFixture]
	public class NameFormatterTests
	{
		[Test]
		[TestCase("Should_change_underscore_to_space", "Should change underscore to space")]
		[TestCase("Should_change_double_underscore_to__colon", "Should change double underscore to: colon")]
		[TestCase("It_s_Paul_s_cat", "It's Paul's cat")]
		public void Should_format_text(string input, string expectedOutput)
		{
			Assert.That(NameFormatter.Format(input), Is.EqualTo(expectedOutput));
		}
	}
}
