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

		private void Given_template_method()
		{
		}

		private void When_template_method()
		{
		}

		private void Then_template_method()
		{
		}
	}
}