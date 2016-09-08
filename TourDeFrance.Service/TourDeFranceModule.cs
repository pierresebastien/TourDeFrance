using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using SimpleStack.Orm;
using SimpleStack.Orm.PostgreSQL;
using TourDeFrance.Core;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Tools.DataBase;
using Module = ASK.ServEasy.Module;

namespace TourDeFrance.Service
{
	public class TourDeFranceModule : Module
	{
		protected override void Initializing()
		{
		}

		protected override void Starting()
		{
			ApplicationConfig config = ApplicationConfig.FromFile();
			IDialectProvider dialectProvider;
			switch (config.DatabaseType)
			{
				case DatabaseType.PostgreSQL:
					dialectProvider = new PostgreSQLDialectProvider();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			Setup setup = new Setup(dialectProvider);
			setup.Initialize(config);
			setup.InitializeContext();
			Context.Current.User = Context.Current.UserRepository.GetAuthenticatedUser(Constants.SYSTEM_USERNAME);

			foreach (IProcess process in setup.Container.Resolve<IEnumerable<IProcess>>())
			{
				if (process.MustRun)
				{
					AddThread(new ProcessModuleThread(setup, process));
				}
			}
		}

		protected override void Stopping()
		{
			foreach (var t in Threads.ToList())
			{
				t.Stop();
				RemoveThread(t);
			}
		}

		protected override void Running()
		{
			Sleep(TimeSpan.MaxValue);
		}
	}
}
