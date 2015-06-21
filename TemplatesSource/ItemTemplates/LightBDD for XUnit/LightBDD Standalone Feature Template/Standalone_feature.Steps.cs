using LightBDD;
using LightBDD.Coordination;
using Xunit;

namespace $rootnamespace$
{
	public partial class $safeitemname$
	{
		private readonly BDDRunner _runner;

		public $safeitemname$()
		{
			_runner = BDDRunnerFactory.GetRunnerFor(GetType(), () => new ConsoleProgressNotifier());
		}

		private void Template_method()
		{
			ScenarioAssert.Ignore("Not implemented yet");
		}
	}
}