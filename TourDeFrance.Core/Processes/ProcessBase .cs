using System;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Core.Processes
{
	public abstract class ProcessBase : IProcess
	{
		public string Name { get; private set; }

		public TimeSpan LoopDelay { get; protected set; }

		public TimeSpan WatchdogDelay { get; protected set; }

		public string Schedule { get; set; }

		public abstract bool MustRun { get; }

		protected Setup Setup { get; private set; }

		protected ProcessBase(string name)
		{
			Name = name;
			LoopDelay = TimeSpan.FromMinutes(5);
			WatchdogDelay = TimeSpan.FromMinutes(1);
			Schedule = "* * * * *";
		}

		public virtual void Initializing(Setup setup)
		{
			Setup = setup;
		}

		public virtual void Starting()
		{
		}

		public virtual void Stopping()
		{
		}

		public virtual void Running(Func<bool> watchdog)
		{
		}

		public virtual void Dispose(bool disposing)
		{
		}
	}
}
