using System;
using System.Web;
using Autofac;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using ServiceStack.CacheAccess;
using SimpleStack.Orm;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Core
{
	public class Context
	{
		[ThreadStatic]
		private static Context _noHttpContext;

		internal Context(IContainer container)
		{
			Container = container;
			ApplicationConfig = container.Resolve<ApplicationConfig>();
			DialectProvider = container.Resolve<IDialectProvider>();
			Config = container.Resolve<Config>();
			EventDispatcher = container.Resolve<EventDispatcher>();
			Cache = container.Resolve<ICacheClient>();
			EmailSender = container.Resolve<IEmailSender>();
		}

		public static Context Current
		{
			get
			{
				Context ctx;

				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					if (_noHttpContext == null)
					{
						throw new ApplicationException("Context is not initialized");
					}
					ctx = _noHttpContext;
				}
				else
				{
					ctx = HttpContext.Current.Session["ctx"] as Context;
					if (ctx == null)
					{
						throw new ApplicationException("Context is not initialized");
					}
				}
				return ctx;
			}
			internal set
			{
				if (value != null)
				{
					if (HttpContext.Current == null || HttpContext.Current.Session == null)
					{
						_noHttpContext = value;
					}
					else
					{
						HttpContext.Current.Session["ctx"] = value;
					}
				}
			}
		}

		public static bool IsContextInitialized
		{
			get
			{
				Context ctx;
				if (HttpContext.Current == null || HttpContext.Current.Session == null)
				{
					ctx = _noHttpContext;
				}
				else
				{
					ctx = HttpContext.Current.Session["ctx"] as Context;
				}
				return ctx != null;
			}
		}

		internal ApplicationConfig ApplicationConfig { get; private set; }

		public AuthenticatedUser User { get; set; }

		public AuthenticatedUser RealUser { get; set; }

		public IContainer Container { get; private set; }

		public IDialectProvider DialectProvider { get; private set; }

		public Config Config { get; private set; }

		public EventDispatcher EventDispatcher { get; private set; }

		public ICacheClient Cache { get; private set; }

		public IEmailSender EmailSender { get; private set; }

		#region Repositories

		public IDrinkRepository DrinkRepository => Container.Resolve<IDrinkRepository>();

		public IStageRepository StageRepository => Container.Resolve<IStageRepository>();

		public IUserRepository UserRepository => Container.Resolve<IUserRepository>();

		public IRaceRepository RaceRepository => Container.Resolve<IRaceRepository>();

		public IPlayerRepository PlayerRepository => Container.Resolve<IPlayerRepository>();

		public ITeamRepository TeamRepository => Container.Resolve<ITeamRepository>();

		public IGameRepository GameRepository => Container.Resolve<IGameRepository>();

		public IRiderRepository RiderRepository => Container.Resolve<IRiderRepository>();

		public IConfigurationRepository ConfigurationRepository => Container.Resolve<IConfigurationRepository>();

		public IEmailTemplateRepository EmailTemplateRepository => Container.Resolve<IEmailTemplateRepository>();

		public ISearchHistoryRepository SearchHistoryRepository => Container.Resolve<ISearchHistoryRepository>();

		#endregion

		public void LogOff()
		{
			User = null;
			RealUser = null;
		}

		public T TryResolve<T>() where T : class 
		{
			T t;
			return Container.TryResolve(out t) ? t : null;
		}
	}
}