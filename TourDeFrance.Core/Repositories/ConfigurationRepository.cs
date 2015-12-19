using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using log4net;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using TourDeFrance.Core.Tools.Cache;
using Autofac;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public class ConfigurationRepository : BaseRepository, IConfigurationRepositoryExtended
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ConfigurationRepository));

		[InvalidateCache(types: new[] { typeof(DbConfig) }, typeArgumentOrders: new[] { 0 })]
		public DbConfig SetValue(string key, string value)
		{
			DbConfig config;
			string oldValue;

			using(var scope =  new TransactionScope())
			{
				config = GetConfigByKey(key);
				if(!string.IsNullOrWhiteSpace(config.ValidationRegex))
				{
					var regex = new Regex(config.ValidationRegex);
					if(!regex.IsMatch(value))
					{
						throw new TourDeFranceException($"Incorrect value for key '{key}'");
					}
				}
				oldValue = config.Value;
				if (oldValue != value)
				{
					config.Value = value;
					config.BeforeUpdate();
					scope.Connection.Update<DbConfig>(config);
				}
				scope.Complete();
			}

			if(oldValue != value)
			{
				try
				{
					RemoveKeysFromCache($"{typeof (DbConfig).Name}:{key}");
					InitValue(key);
				}
				catch(Exception)
				{
					Logger.WarnFormat("Error while setting value '{0}' to config key '{1}' => rollback", value, key);
					using(var scope = new TransactionScope())
					{
						config.Value = value;
						config.BeforeUpdate();
						scope.Connection.Update<DbConfig>(config);
						scope.Complete();
					}
					RemoveKeysFromCache($"{typeof (DbConfig).Name}:{key}");
					InitValue(key);
				}
			}

			return config;
		}

		public T GetValue<T>(string key)
		{
			return GetConfigByKey(key).FromDatabaseValue<T>();
		}

		[Cache(ArgumentOrder = 0)]
		public virtual DbConfig GetConfigByKey(string key, bool throwIfNotExist = true)
		{
			key.EnsureIsNotEmpty("Config key can't be null or empty");
			using (var scope = new TransactionScope())
			{
				var result = scope.Connection.FirstOrDefault<DbConfig>(x => x.Key == key);
				if (result == null && throwIfNotExist)
				{
					throw new NotFoundException($"Config with key '{key}' not found");
				}
				scope.Complete();
				return result;
			}
		}

		public IEnumerable<DbConfig> GetConfigs()
		{
			using (var scope = new TransactionScope())
			{
				IList<DbConfig> result = scope.Connection.Select<DbConfig>().ToList();
				scope.Complete();
				return result;
			}
		}

		public void InitValue(string key)
		{
			switch (key)
			{
				case "TempFolder":
					if (!Directory.Exists(Config.TempFolder))
					{
						Directory.CreateDirectory(Config.TempFolder);
					}
					break;
				case "IndexFolder":
					if (Config.UseLucene)
					{
						if (!Directory.Exists(Config.IndexFolder))
						{
							Directory.CreateDirectory(Config.IndexFolder);
						}
						Context.Current.Container.Resolve<ILuceneRepository>().Initialize();
					}
					break;
			}
		}
	}
}