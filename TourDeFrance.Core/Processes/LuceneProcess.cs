using Autofac;
using System;
using TourDeFrance.Core.Logging;
using TourDeFrance.Core.Repositories.Interfaces;

namespace TourDeFrance.Core.Processes
{
	public class LuceneProcess : ProcessBase
	{
		private static readonly ILog Logger = LogProvider.For<LuceneProcess>();

		public LuceneProcess() : base("Indexation")
		{
			WatchdogDelay = TimeSpan.FromMinutes(30);
			LoopDelay = TimeSpan.FromSeconds(5);
		}

		public override bool MustRun => Context.Current.Config.UseLucene;

		public override void Starting()
		{
			// TODO : check last log history in lucene and in redis
		}

		public override void Running(Func<bool> watchdog)
		{
			Logger.Debug("Looking for indexation jobs");
			Context.Current.Container.Resolve<ILuceneRepository>().ProcessWaitingJobs();
		}
	}
}
