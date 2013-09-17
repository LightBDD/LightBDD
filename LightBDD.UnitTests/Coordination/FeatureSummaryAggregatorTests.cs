using System;
using System.IO;
using System.Linq;
using LightBDD.Coordination;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.Results.Implementation;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests.Coordination
{
	[TestFixture]
	public class FeatureSummaryAggregatorTests
	{
		private FeatureSummaryAggregator _subject;
		private IResultFormatter _formatter;
		private string _filePath;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_filePath = Guid.NewGuid().ToString();
			_formatter = MockRepository.GenerateMock<IResultFormatter>();
			_subject = new FeatureSummaryAggregator(_formatter, _filePath);
		}

		#endregion

		[Test]
		public void Should_add_and_save_results()
		{
			const string expectedText = "some expected text";
			var feature = new FeatureResult("name", "description", "label");
			try
			{
				_formatter.Stub(f => f.Format(Arg<IFeatureResult[]>.Matches(l => l.Contains(feature)))).Return(expectedText);
				_subject.AddFeature(feature);
				_subject.Finished();
				Assert.That(File.ReadAllText(_filePath), Is.EqualTo(expectedText));
			}
			finally
			{
				File.Delete(_filePath);
			}
		}
	}
}
