using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using LightBDD.Results;
using LightBDD.Results.Formatters;
using LightBDD.Results.Implementation;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
	[TestFixture]
	public class TestResultsSummaryTests
	{
		private TestResultsSummary _subject;
		private IResultFormatter _formatter;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_formatter = MockRepository.GenerateMock<IResultFormatter>();
			_subject = new TestResultsSummary(_formatter);
		}

		#endregion

		[Test]
		public void Should_add_results()
		{
			var featureResult = new FeatureResult(string.Empty, string.Empty, string.Empty);
			_subject.AddResult(featureResult);

			Assert.That(_subject.Results, Is.EqualTo(new[] { featureResult }));
		}

		[Test]
		public void Should_add_result_be_thread_safe()
		{
			var results = new List<FeatureResult>();

			for (int i = 0; i < 1000; ++i)
				results.Add(new FeatureResult(
					i.ToString(CultureInfo.InvariantCulture),
					i.ToString(CultureInfo.InvariantCulture),
					i.ToString(CultureInfo.InvariantCulture)));

			results.AsParallel().ForAll(r => _subject.AddResult(r));

			foreach (var result in results)
				Assert.That(_subject.Results.Contains(result), string.Format("Result {0} is missing", result.Name));
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