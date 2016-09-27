using ASK.ServEasy;
using TourDeFrance.Core;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Service
{
	public class ProcessModuleThread : ModuleThread
	{
		private readonly Setup _setup;
		private readonly IProcess _process;

		public ProcessModuleThread(Setup setup, IProcess process)
			: base(process.Name)
		{
			_setup = setup;
			_process = process;

			WatchdogDelay = process.WatchdogDelay;

			if (!string.IsNullOrEmpty(_process.Schedule))
			{
				Schedule = _process.Schedule;
			}
		}

		protected override void Initializing()
		{
			_process.Initializing(_setup);
		}

		protected override void Starting()
		{
			_setup.InitializeContext();
			Context.Current.User = Context.Current.UserRepository.GetAuthenticatedUser(Constants.ADMIN_USERNAME);
			_process.Starting();
		}

		protected override void Stopping()
		{
			_process.Stopping();
		}

		protected override void Running()
		{
			_process.Running(() =>
			{
				ResetWatchdog();
				return MustRun;
			});

			if (MustRun && string.IsNullOrEmpty(_process.Schedule))
			{
				Sleep(_process.LoopDelay);
			}
		}

		protected override void Dispose(bool disposing)
		{
			_process.Dispose(disposing);
		}
	}
}
