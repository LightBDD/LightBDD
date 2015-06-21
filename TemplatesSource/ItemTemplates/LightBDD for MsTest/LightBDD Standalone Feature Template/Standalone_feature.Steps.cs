using LightBDD;
using LightBDD.Notification;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
			Assert.Inconclusive("Not implemented yet");
		}
	}
}