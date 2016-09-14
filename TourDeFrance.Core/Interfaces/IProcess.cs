using System;

namespace TourDeFrance.Core.Interfaces
{
	public interface IProcess
	{
		string Name { get; }

		TimeSpan LoopDelay { get; }

		TimeSpan WatchdogDelay { get; }

		string Schedule { get; set; }

		bool MustRun { get; }

		void Initializing(Setup setup);

		void Starting();

		void Stopping();

		void Running(Func<bool> watchdog);

		void Dispose(bool disposing);
	}
}
