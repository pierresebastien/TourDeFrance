using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Repositories;
using TourDeFrance.Core.Tools;
using TourDeFrance.Core.Tools.Cache;
using ServiceStack.Redis;
using System.IO;
using SimpleStack.Orm;
using TourDeFrance.Core.Logging;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core
{
	// TODO: to remember
	// review cache (check possibility with roslyn instead of using fody)
	// unit test (nunit)
	// wixsharp setup
	// script for debian package?
	// improve autofac usage -> module??, factories?? (module in test to change default autofac registration)
	// review owner mechanism in all repo
	// improve usage of const in repositories
	// reduce usage of var
	// !! date => to universal time
	// !! json object in database
	// all method in repositories => virtual
	// usage of required on non nullable field in database??
	// ensure 18 years old for each birth date??? or ensure is in past
	// investigate : https://github.com/damianh/LibLog
	// investigate : https://code.google.com/p/elmah/
	// investigate : https://github.com/turquoiseowl/i18n/tree/v2.0 or https://github.com/dotnetwise/i18N-Complete
	// usage of citext in dapper or simplestack.orm
	// add support for : https://coveralls.io/
	// add support for : https://scan.coverity.com/
	// add support for : https://travis-ci.org/
	// review servicestack licence
	public class Setup
	{
		private static readonly ILog Logger = LogProvider.For<Setup>();

		public Setup(IDialectProvider dialectProvider)
		{
			DialectProvider = dialectProvider;

			DataBaseManagerType = typeof (ScriptDatabaseManager);
			EmailSenderType = typeof(EmailSender);
			ConfigurationRepositoryType = typeof(ConfigurationRepository);
			RiderRepositoryType = typeof(RiderRepository);
			StageRepositoryType = typeof (StageRepository);
			DrinkRepositoryType = typeof (DrinkRepository);
			UserRepositoryType = typeof (UserRepository);
			RaceRepositoryType = typeof (RaceRepository);
			PlayerRepositoryType = typeof (PlayerRepository);
			TeamRepositoryType = typeof (TeamRepository);
			GameRepositoryType = typeof (GameRepository);
			EmailTemplateRepositoryType = typeof(EmailTemplateRepository);
			SearchHistoryRepositoryType = typeof (SearchHistoryRepository);
		}

		public IContainer Container { get; private set; }
		public IDialectProvider DialectProvider { get; set; }

		public Type DataBaseManagerType { get; set; }
		public Type EmailSenderType { get; set; }
		public Type ConfigurationRepositoryType { get; set; }
		public Type RiderRepositoryType { get; set; }
		public Type StageRepositoryType { get; set; }
		public Type DrinkRepositoryType { get; set; }
		public Type UserRepositoryType { get; set; }
		public Type RaceRepositoryType { get; set; }
		public Type PlayerRepositoryType { get; set; }
		public Type TeamRepositoryType { get; set; }
		public Type GameRepositoryType { get; set; }
		public Type EmailTemplateRepositoryType { get; set; }
		public Type SearchHistoryRepositoryType { get; set; }

		public void Initialize(ApplicationConfig config)
		{
			// TODO: or all assemblies ?
			Assembly[] assemblies =
				Directory.GetFiles(config.ApplicationPath, "*.dll")
						 .Where(x => Path.GetFileName(x).StartsWith("TourDeFrance")).Select(Assembly.LoadFrom).ToArray();

			var b = new ContainerBuilder();
			try
			{
				b.RegisterInstance(DialectProvider).AsSelf().AsImplementedInterfaces().SingleInstance();
				b.RegisterInstance(config).AsSelf().AsImplementedInterfaces().SingleInstance();
				b.RegisterType<Config>().AsSelf().AsImplementedInterfaces().SingleInstance();
				b.RegisterType<EventDispatcher>().AsSelf().AsImplementedInterfaces().SingleInstance();
				b.RegisterType(DataBaseManagerType).AsImplementedInterfaces().SingleInstance();
				b.RegisterType(EmailSenderType).AsImplementedInterfaces().SingleInstance();

				if (string.IsNullOrWhiteSpace(config.RedisHost))
				{
					b.RegisterType<MemoryCacheClient>().AsImplementedInterfaces().SingleInstance();
					b.RegisterType<InMemoryPrioritizedStack>().AsImplementedInterfaces().SingleInstance();
				}
				else{
					IRedisClientsManager redisClientManager = new PooledRedisClientManager(0, config.RedisHost);
					using (IRedisClient client = redisClientManager.GetClient())
					{
						client.RemoveAll(client.SearchKeys(Constants.BASE_CACHE_KEY + "*"));
					}
					b.RegisterInstance(redisClientManager).AsImplementedInterfaces().SingleInstance();
					b.RegisterType<RedisClientManagerCacheClient>().AsImplementedInterfaces().SingleInstance();
					b.RegisterType<RedisPrioritizedStack>().AsImplementedInterfaces().SingleInstance();
				}

				// TODO: repository single instance ???
				b.RegisterType(ConfigurationRepositoryType).AsImplementedInterfaces();
				b.RegisterType(RiderRepositoryType).AsImplementedInterfaces();
				b.RegisterType(StageRepositoryType).AsImplementedInterfaces();
				b.RegisterType(DrinkRepositoryType).AsImplementedInterfaces();
				b.RegisterType(UserRepositoryType).AsImplementedInterfaces();
				b.RegisterType(RaceRepositoryType).AsImplementedInterfaces();
				b.RegisterType(PlayerRepositoryType).AsImplementedInterfaces();
				b.RegisterType(TeamRepositoryType).AsImplementedInterfaces();
				b.RegisterType(GameRepositoryType).AsImplementedInterfaces();
				b.RegisterType(EmailTemplateRepositoryType).AsImplementedInterfaces();
				b.RegisterType(SearchHistoryRepositoryType).AsImplementedInterfaces();

				if (config.UseLucene)
				{
					b.RegisterType<LuceneRepository>().AsImplementedInterfaces();
				}

				b.RegisterAssemblyTypes(assemblies).As<IProcess>().SingleInstance();
				b.RegisterAssemblyTypes(assemblies).As<IEventListener>();
				b.RegisterAssemblyModules<ITourDeFranceModule>(assemblies);

				Container = b.Build();
			}
			catch (ReflectionTypeLoadException reflectionTypeLoadException)
			{
				Logger.FatalException("Error while loading Autofac components", reflectionTypeLoadException);
				foreach (var e in reflectionTypeLoadException.LoaderExceptions)
				{
					Logger.FatalException("Reflection error", e);
				}
				throw;
			}

			Container.Resolve<IDatabaseManager>().SetupDatabase();

			ContainerBuilder updater = new ContainerBuilder();
			using (var connection = DialectProvider.CreateConnection(config.ConnectionString))
			{
				// register rights or other things from database
			}
			updater.Update(Container);

			InitializeContext();
			foreach (IInitializable initializable in Container.Resolve<IEnumerable<IInitializable>>().OrderBy(x => x.Order))
			{
				initializable.Initialize();
			}
		}

		public void InitializeContext()
		{
			Context.Current = new Context(Container);
		}
	}
}