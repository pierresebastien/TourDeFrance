using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dapper;
using SimpleStack.Orm;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Logging;

namespace TourDeFrance.Core.Tools.DataBase
{
	public class ScriptDatabaseManager : IDatabaseManager
	{
		private static readonly Regex VersionRegex = new Regex(@"^(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)$", RegexOptions.Compiled);

		protected readonly IDialectProvider DialectProvider;
		protected readonly ApplicationConfig Config;
		protected readonly string ScriptPath;

		private static readonly ILog Logger = LogProvider.For<ScriptDatabaseManager>();

		public ScriptDatabaseManager(IDialectProvider dialectProvider, ApplicationConfig config)
		{
			DialectProvider = dialectProvider;
			Config = config;
			string baseScriptPath = Path.Combine(config.ApplicationPath, "Sql");
			ScriptPath = Path.Combine(baseScriptPath, config.DatabaseType.ToString());
		}

		public virtual void SetupDatabase()
		{
			Logger.Debug($"Script path : {ScriptPath}");

			using (var scope = new TransactionScope(DialectProvider, Config))
			{
				Init(scope);

				string currentVersion = "0.0.0";
				if (scope.Connection.TableExists<DbVersion>())
				{
					DbVersion dbVersion =
							scope.Connection.Select<DbVersion>(x =>
							{
								x.OrderByDescending(y => y.Major).ThenByDescending(y => y.Minor).ThenByDescending(y => y.Patch);
								return x;
							}).First();
					currentVersion = $"{dbVersion.Major}.{dbVersion.Minor}.{dbVersion.Patch}";
					Logger.Info($"Database already exists (Version {currentVersion})");
				}

				foreach (
					string versionScriptDirectory in
						Directory.GetDirectories(ScriptPath)
							.Where(x => VersionRegex.IsMatch(Path.GetFileName(x)))
							.Where(x => string.Compare(Path.GetFileName(x), currentVersion, StringComparison.Ordinal) > 0)
							.OrderBy(Path.GetFileName))
				{
					string versionString = Path.GetFileName(versionScriptDirectory);

					Logger.Info($"Upgrading Database ... (Version {versionString})");

					foreach (string scriptFile in Directory.GetFiles(versionScriptDirectory).OrderBy(Path.GetFileName))
					{
						Logger.Info($"Executing SQL Script : {Path.GetFileName(scriptFile)}");
						scope.Connection.Execute(File.ReadAllText(scriptFile));
					}

					SpecificVersionUpgradeDatabaseCode(scope, versionString);

					var match = VersionRegex.Match(versionString);
					scope.Connection.Insert(new DbVersion
					{
						Major = int.Parse(match.Groups["major"].Value),
						Minor = int.Parse(match.Groups["minor"].Value),
						Patch = int.Parse(match.Groups["patch"].Value)
					});
					Logger.Info($"Database Upgraded to version {versionString}");
				}

				Finish(scope);
				scope.Complete();
			}
			Logger.Info("Database Upgrade completed");
		}

		protected virtual void Init(TransactionScope scope)
		{
			switch (Config.DatabaseType)
			{
				case DatabaseType.SQLite:
					scope.Connection.Execute("PRAGMA foreign_keys = OFF;");
					break;
			}
		}

		protected virtual void Finish(TransactionScope scope)
		{
			switch (Config.DatabaseType)
			{
				case DatabaseType.SQLite:
					scope.Connection.Execute("PRAGMA foreign_keys = ON;");
					break;
			}
		}

		protected virtual void SpecificVersionUpgradeDatabaseCode(TransactionScope scope, string version)
		{
			switch (version)
			{
				case "1.0.0":
					string basePath = Path.Combine(Path.GetTempPath(), "TourDeFrance");

					string temporaryPath = Path.Combine(basePath, "Temp");
					scope.Connection.UpdateAll(new DbConfig {Value = temporaryPath, DefaultValue = temporaryPath},
						x => x.Key == "TempFolder", x => new {x.Value, x.DefaultValue});

					string lucenePath = Path.Combine(basePath, "Lucene");
					scope.Connection.UpdateAll(new DbConfig {Value = lucenePath, DefaultValue = lucenePath},
						x => x.Key == "IndexFolder", x => new {x.Value, x.DefaultValue});
					break;
			}
		}
	}
}
