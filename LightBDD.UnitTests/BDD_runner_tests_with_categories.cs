using System.Linq;
using LightBDD.Notification;
using LightBDD.UnitTests.Helpers;
using NUnit.Framework;
using Rhino.Mocks;

namespace LightBDD.UnitTests
{
    [TestFixture]
    [FeatureDescription("Runner tests description")]
    [Label("Ticket-1")]
    [Category("Category A"), Category("Category B"), FeatureCategory("Category C")]
    public class BDD_runner_tests_with_categories : SomeSteps
    {
        private AbstractBDDRunner _subject;

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _subject = new TestableBDDRunner(GetType(), MockRepository.GenerateMock<IProgressNotifier>());
        }

        #endregion

        [Test]
        public void Should_capture_all_categories()
        {
            Assert.That(_subject.Result.Categories.ToArray(), Is.EquivalentTo(new[] { "Category A", "Category B", "Category C" }));
        }
    }
}