using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using log4net.Config;
using ServiceStack.CacheAccess;
using NUnit.Framework;
using SimpleStack.Orm;
using SimpleStack.Orm.PostgreSQL;
using SimpleStack.Orm.Sqlite;
using TourDeFrance.Core;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using Dapper;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Tools.DataBase;
using TourDeFrance.Tests.Tools;

namespace TourDeFrance.Tests
{
	public abstract partial class BaseRepositoryTests
	{
		protected abstract DatabaseType DatabaseType { get; }

		protected abstract IDialectProvider DialectProvider { get; }

		protected abstract string ConnectionString { get; }

		protected abstract void CleanDatabase();

		protected abstract Type DatabaseManagerType { get; }

		protected const string DatabaseName = "tour_de_france";

		protected const string SchemaName = "test";

		protected IContainer Container;

		protected ICacheClient Cache => Context.Current.Cache;

		protected AuthenticatedUser CurrentUser => Context.Current.User;

		protected IDrinkRepository DrinkRepository => Context.Current.DrinkRepository;

		protected IStageRepository StageRepository => Context.Current.StageRepository;

		protected IUserRepository UserRepository => Context.Current.UserRepository;

		protected IRaceRepository RaceRepository => Context.Current.RaceRepository;

		protected IPlayerRepository PlayerRepository => Context.Current.PlayerRepository;

		protected ITeamRepository TeamRepository => Context.Current.TeamRepository;

		protected IGameRepository GameRepository => Context.Current.GameRepository;

		protected IRiderRepository RiderRepository => Context.Current.RiderRepository;

		protected IConfigurationRepository ConfigurationRepository => Context.Current.ConfigurationRepository;

		protected IEmailTemplateRepository EmailTemplateRepository => Context.Current.EmailTemplateRepository;

		protected ISearchHistoryRepository SearchHistoryRepository => Context.Current.SearchHistoryRepository;

		[SetUp]
		public virtual void Setup()
		{
			BasicConfigurator.Configure();
			Initialize();
		}

		[TearDown]
		public virtual void Cleanup()
		{
			Container.Dispose();
			Container = null;
		}

		private void Initialize()
		{
			CleanDatabase();
			ApplicationConfig config = new ApplicationConfig
			{
				DatabaseType = DatabaseType,
				ConnectionString = ConnectionString,
				RedisHost = "", // TODO: use redis in test ???
				UseLucene = false
			};
			var setup = new Setup(DialectProvider) {DataBaseManagerType = DatabaseManagerType };
			setup.Initialize(config);
			Container = setup.Container;
			setup.InitializeContext();
			AsAdmin();

			UserRepository.CreateUser("SimpleUser", "Simple", "User", Gender.Male, "simpleuser@tourdefrance.com", null, null,
				null, false, "password", false, false);
			UserRepository.CreateUser("OtherAdmin", "Other", "Admin", Gender.Female, "otheradmin@tourdefrance.com", null, null,
				null, true, "password", false, false);
		}

		protected void AsSimpleUser()
		{
			Context.Current.User = UserRepository.GetAuthenticatedUser("SimpleUser");
		}

		protected void AsAdmin()
		{
			Context.Current.User = UserRepository.GetAuthenticatedUser("Admin");
		}

		protected void AsOtherAdmin()
		{
			Context.Current.User = UserRepository.GetAuthenticatedUser("OtherAdmin");
		}

		#region Assert

		private void AssertTwoObjectAreEquals<T>(T o1, T o2)
		{
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
				{
					AssertTwoDatesAreEqual((DateTime?)propertyInfo.GetValue(o1, null), (DateTime?)propertyInfo.GetValue(o2, null));
				}
				else
				{
					Assert.AreEqual(propertyInfo.GetValue(o1, null), propertyInfo.GetValue(o2, null));
				}
			}
		}

		protected void AssertTwoDatesAreEqual(DateTime? date1, DateTime? date2)
		{
			Assert.AreEqual(date1.HasValue, date2.HasValue);
			if (date1.HasValue && date2.HasValue)
			{
				Assert.AreEqual(date1.Value.Year, date2.Value.Year);
				Assert.AreEqual(date1.Value.Month, date2.Value.Month);
				Assert.AreEqual(date1.Value.Day, date2.Value.Day);
				Assert.AreEqual(date1.Value.Hour, date2.Value.Hour);
				Assert.AreEqual(date1.Value.Minute, date2.Value.Minute);
				Assert.AreEqual(date1.Value.Second, date2.Value.Second);
			}
		}

		#endregion

		#region Cache

		protected T GetCacheObject<T>(object key)
		{
			return Cache.Get<T>(key.GenerateCacheKey<T>());
		}

		protected void CheckCache<T, TKey>(T t, TKey key, Func<TKey, T> getObject)
		{
			string cacheKey = key.GenerateCacheKey<T>();
			Assert.IsNull(Cache.Get<T>(cacheKey));
			T getT = getObject(key);
			AssertTwoObjectAreEquals(getT, t);
			var cacheT = Cache.Get<T>(cacheKey);
			Assert.IsNotNull(cacheT);
			AssertTwoObjectAreEquals(cacheT, t);
		}

		protected void CheckCache<T, TKey>(T t, TKey key)
		{
			string cacheKey = key.GenerateCacheKey<T>();
			var cacheT = Cache.Get<T>(cacheKey);
			Assert.IsNotNull(cacheT);
			AssertTwoObjectAreEquals(cacheT, t);
		}

		protected IList<T> CheckCacheOnList<T, TKey>(string cacheName, TKey key, Func<TKey, IEnumerable<T>> getObject)
		{
			string cacheKey = $"{cacheName}:{key}".FormatCacheKey();
			Assert.IsNull(Cache.Get<IEnumerable<T>>(cacheKey));
			IList<T> getT = getObject(key).ToList();
			IList<T> cacheT = Cache.Get<IEnumerable<T>>(cacheKey).ToList();
			Assert.IsNotNull(cacheT);
			AssertTwoListOfObjectAreEquals(cacheT, getT);
			return getT;
		}

		protected void AssertTwoListOfObjectAreEquals<T>(IList<T> lo1, IList<T> lo2)
		{
			Assert.AreEqual(lo1.Count, lo2.Count);
			for (int i = 0; i < lo1.Count; i++)
			{
				T o1 = lo1[i];
				T o2 = lo2[i];
				AssertTwoObjectAreEquals(o1, o2);
			}
		}

		#endregion
	}

	// TODO: in memory not supported for the moment because we create a lot of new connections
	// => put on disk + review with shared memory db
	[TestFixture]
	public class RepositoryTestsBaseSqLite : BaseRepositoryTests
	{
		private static readonly IDialectProvider Provider = new SqliteDialectProvider();

		protected override DatabaseType DatabaseType => DatabaseType.SQLite;

		protected override IDialectProvider DialectProvider => Provider;

		protected override string ConnectionString => ":memory:";

		protected override void CleanDatabase()
		{
		}

		protected override Type DatabaseManagerType => typeof(OrmDatabaseManager);
	}

	[TestFixture]
	public class RepositoryTestsBasePostgreSql : BaseRepositoryTests
	{
		private static readonly IDialectProvider Provider = new PostgreSQLDialectProvider();

		protected static readonly string BaseConnectionString = $"Server=127.0.0.1;Port=5432;User Id=tourdefrance;Password=password;Pooling=false;CommandTimeout=30;Database={SchemaName};";

		protected override DatabaseType DatabaseType => DatabaseType.PostgreSQL;

		protected override IDialectProvider DialectProvider => Provider;

		protected override string ConnectionString => $"{BaseConnectionString}SearchPath={DatabaseName};";

		protected override void CleanDatabase()
		{
			using (OrmConnection connection = DialectProvider.CreateConnection(BaseConnectionString))
			{
				string queryExist = $"SELECT schema_name FROM information_schema.schemata WHERE schema_name = '{SchemaName}'";
				if (connection.Query<string>(queryExist).Any())
				{
					connection.Execute($"DROP SCHEMA {SchemaName} CASCADE");
				}
				connection.Execute($"CREATE SCHEMA {SchemaName}");
			}
		}

		protected override Type DatabaseManagerType => typeof(TestScriptDatabaseManager);
	}
}
