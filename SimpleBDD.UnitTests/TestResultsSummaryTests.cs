using System;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using SimpleBDD.Results;
using SimpleBDD.Results.Formatters;
using SimpleBDD.Results.Implementation;

namespace SimpleBDD.UnitTests
{
	[TestFixture]
	public class TestResultsSummaryTests
	{
		private TestResultsSummary _subject;
		private IResultFormatter _formatter;

		[SetUp]
		public void SetUp()
		{
			_formatter = MockRepository.GenerateMock<IResultFormatter>();
			_subject = new TestResultsSummary(_formatter);
		}

		[Test]
		public void Should_add_results()
		{
			var featureResult = new FeatureResult(string.Empty, string.Empty);
			_subject.AddResult(featureResult);

			Assert.That(_subject.Results, Is.EqualTo(new[] { featureResult }));
		}

		[Test]
		public void Should_save_summary()
		{
			string filePath = Guid.NewGuid().ToString();
			const string expectedText = "some expected text";
			try
			{
				_formatter.Stub(f => f.Format(Arg<IFeatureResult[]>.Is.Anything)).Return(expectedText);
				_subject.SaveSummary(filePath);
				Assert.That(File.ReadAllText(filePath), Is.EqualTo(expectedText));
			}
			finally
			{
				File.Delete(filePath);
			}

		}
	}
}