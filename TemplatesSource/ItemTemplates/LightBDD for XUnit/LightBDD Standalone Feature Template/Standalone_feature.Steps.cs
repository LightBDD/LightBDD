using LightBDD;
using LightBDD.Notification;
using Xunit;
using Xunit.Abstractions;

namespace $rootnamespace$
{
	public partial class $safeitemname$
	{
		private readonly BDDRunner _runner;
		private readonly ITestOutputHelper _output;

		public $safeitemname$(ITestOutputHelper output)
		{
			_output = output;
			_runner = BDDRunnerFactory.GetRunnerFor(GetType(), () => new DelegatingProgressNotifier(new XUnitOutputProgressNotifier(_output), SimplifiedConsoleProgressNotifier.GetInstance()));
		}

		private void Template_method()
		{
			ScenarioAssert.Ignore("Not implemented yet");
		}
	}
}