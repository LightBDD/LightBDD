using Microsoft.VisualStudio.TestTools.UnitTesting;
using LightBDD;
using LightBDD.Notification;

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
		}
	}
}