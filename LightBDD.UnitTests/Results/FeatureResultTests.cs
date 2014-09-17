using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LightBDD.Results;
using LightBDD.Results.Implementation;
using NUnit.Framework;

namespace LightBDD.UnitTests.Results
{
	[TestFixture]
	public class FeatureResultTests
	{
		private FeatureResult _subject;

		#region Setup/Teardown

		[SetUp]
		public void SetUp()
		{
			_subject = new FeatureResult("name", "desc", "label");
		}

		#endregion

		[Test]
		public void Should_add_result_be_thread_safe()
		{
			var results = new List<ScenarioResult>();

			for (int i = 0; i < 1000; ++i)
				results.Add(new ScenarioResult(i.ToString(CultureInfo.InvariantCulture), Enumerable.Empty<IStepResult>(), null));

			results.AsParallel().ForAll(r => _subject.AddScenario(r));

			foreach (var result in results)
				Assert.That(_subject.Scenarios.Contains(result), string.Format("Result {0} is missing", result.Name));
		}
	}
}