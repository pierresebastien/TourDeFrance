using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Dapper;
using ServiceStack.Text;
using SimpleStack.Orm;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Logging;

namespace TourDeFrance.Core.Tools.DataBase
{
	public class OrmDatabaseManager : IDatabaseManager
	{
		private static readonly ILog Logger = LogProvider.For<OrmDatabaseManager>();
		protected static readonly DateTime DefaultDateTime = new DateTime(2000, 1, 1);
		protected static readonly string DefaultUser = "System";

		protected readonly IDialectProvider DialectProvider;
		protected readonly ApplicationConfig Config;
		protected bool DropExistingDatabase;
		protected readonly Version CurrentApplicationVersion;

		public OrmDatabaseManager(IDialectProvider dialectProvider, ApplicationConfig config)
		{
			DialectProvider = dialectProvider;
			Config = config;
			switch (Config.DatabaseType)
			{
				case DatabaseType.PostgreSQL:
					DropExistingDatabase = false;
					break;
				case DatabaseType.SQLite:
					DropExistingDatabase = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			CurrentApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version;
		}

		public virtual void SetupDatabase()
		{
			using (var scope = new TransactionScope(DialectProvider, Config))
			{
				if (!DropExistingDatabase)
				{
					if (scope.Connection.TableExists("databaseversion"))
					{
						DbVersion dbVersion =
							scope.Connection.Select<DbVersion>(x =>
							{
								x.OrderByDescending(y => y.Major).ThenByDescending(y => y.Minor).ThenByDescending(y => y.Patch);
								return x;
							}).First();
						if (CurrentApplicationVersion != new Version(dbVersion.Major, dbVersion.Minor, dbVersion.Patch))
						{
							DropExistingDatabase = true;
						}
					}
				}

				Init(scope);
				CreateTables(scope);
				DefaultInserts(scope);
				CreateViews(scope);
				Finish(scope);
				scope.Complete();
			}
		}

		protected virtual void Init(TransactionScope scope)
		{
			Logger.Info("Init");
			switch (Config.DatabaseType)
			{
				case DatabaseType.PostgreSQL:
					scope.Connection.Execute(@"CREATE EXTENSION IF NOT EXISTS ""uuid-ossp""");
					break;
				case DatabaseType.SQLite:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected virtual void CreateTables(TransactionScope scope)
		{
			Logger.Info("Create tables");
			scope.Connection.CreateTable<DbVersion>(DropExistingDatabase);
			scope.Connection.CreateTable<DbConfig>(DropExistingDatabase);
			scope.Connection.CreateTable<DbUser>(DropExistingDatabase);
			scope.Connection.CreateTable<DbAccessShare>(DropExistingDatabase);
			scope.Connection.CreateTable<DbGlobalMailTemplate>(DropExistingDatabase);
			scope.Connection.CreateTable<DbMailTemplate>(DropExistingDatabase);
			scope.Connection.CreateTable<DbDrink>(DropExistingDatabase);
			scope.Connection.CreateTable<DbSubDrink>(DropExistingDatabase);
			scope.Connection.CreateTable<DbStage>(DropExistingDatabase);
			scope.Connection.CreateTable<DbStageDrink>(DropExistingDatabase);
			scope.Connection.CreateTable<DbRace>(DropExistingDatabase);
			scope.Connection.CreateTable<DbRaceStage>(DropExistingDatabase);
			scope.Connection.CreateTable<DbTeam>(DropExistingDatabase);
			scope.Connection.CreateTable<DbRider>(DropExistingDatabase);
			scope.Connection.CreateTable<DbGame>(DropExistingDatabase);
			scope.Connection.CreateTable<DbPlayer>(DropExistingDatabase);
			scope.Connection.CreateTable<DbGameParticipant>(DropExistingDatabase);
			scope.Connection.CreateTable<DbLog>(DropExistingDatabase);
			scope.Connection.CreateTable<DbScore>(DropExistingDatabase);
		}

		protected virtual void DefaultInserts(TransactionScope scope)
		{
			Logger.Info("Default inserts");

			scope.Connection.Insert(new DbVersion
			{
				Major = CurrentApplicationVersion.Major,
				Minor = CurrentApplicationVersion.Minor,
				Patch = CurrentApplicationVersion.Build
			});

			var saltedHash = new SaltedHash();
			string salt;
			string hash;
			saltedHash.GetHashAndSaltString("password", out hash, out salt);
			DbUser user = new DbUser
			{
				Id = Guid.NewGuid(),
				Gender = Gender.Male,
				Username = "Admin",
				Password = hash,
				Salt = salt,
				FirstName = "John",
				LastName = "Doe",
				Email = "admin@tourdefrancebibitif.com",
				BirthDate = DefaultDateTime,
				Height = null,
				Weight = null,
				IsAdministrator = true,
				IsBlocked = false,
				IsDisabled = false,
				LastPasswordChangeDate = DefaultDateTime,
				NumberOfFailedAttempts = 0,
				RequireNewPasswordAtLogon = false,
				ApiKey = Guid.NewGuid().ToString(),
				CreationDate = DefaultDateTime,
				LastUpdateBy = DefaultUser,
				LastUpdateDate = DefaultDateTime,
				PreviousPasswords =
					new Dictionary<int, Dictionary<string, string>> {{1, new Dictionary<string, string> {{hash, salt}}}}.ToJson()
			};
			scope.Connection.Insert(user);

			#region Config

			CreateConfig(scope, "PublicUri", "Public uri", "Public address of this web application", ConfigType.String, 1,
				"http://localhost/");

			CreateConfig(scope, "DecimalFormat", "Decimal format", "Format to use to display decimals in application",
				ConfigType.String, 100, "#,##0.00");
			CreateConfig(scope, "CultureInfo", "Culture", "Culture to use in application", ConfigType.String, 101, "fr-BE");

			string basePath = Path.Combine(Path.GetTempPath(), "TourDeFrance");
			CreateConfig(scope, "TempFolder", "Temporary folder path", "Path to the temporary folder", ConfigType.String, 200,
				Path.Combine(basePath, "Temp"), dangerous: true);
			CreateConfig(scope, "IndexFolder", "Index folder path", "Path to the lucene index folder", ConfigType.String, 201,
				Path.Combine(basePath, "Lucene"), dangerous: true);

			CreateConfig(scope, "SmtpSenderEmailAddress", "Sender email address",
				"Email address to use to send mail with the application", ConfigType.String, 300, "noreply@tourdefrancebibitif.com",
				validationregex: @"^\S+@\S+$");
			CreateConfig(scope, "SmtpSenderDisplayName", "Sender display name",
				"Display name used when sending mails with the application", ConfigType.String, 301, "Tour de France");
			CreateConfig(scope, "ErrorMailRecipient", "Error email address",
				"Email address used when an error is generated by the application", ConfigType.String, 302,
				"error@tourdefrancebibitif.com", validationregex: @"^\S+@\S+$");

			CreateConfig(scope, "MinUserNameLength", "Username minimum length", "The minimum number of characters in a username",
				ConfigType.Integer, 400, 3.ToString(), validationregex: @"^[1-9][0-9]*$");
			CreateConfig(scope, "NumberOfHourBeforeTokenExpiration", "Number of hours allowed to change password",
				"Number of hours before the expiration of the change password token", ConfigType.Integer, 401, 24.ToString(),
				validationregex: @"^[1-9][0-9]*$");
			CreateConfig(scope, "NumberOfPreviousPasswordsAuthorized", "Number of previous passwords",
				"Number of previous passwords stored", ConfigType.Integer, 402, 3.ToString(), validationregex: @"^[0-9]+$");
			CreateConfig(scope, "PasswordRegexes", "Password rules", "Rules to validate a password", ConfigType.Array, 403,
				new[] {@"^.{8,255}$"}.ToJson());
			CreateConfig(scope, "PasswordErrorMessage", "Password error message",
				"Error message to display when the password does not respect all the rules", ConfigType.String, 404,
				"The password must have a minimum of 8 characters.");
			CreateConfig(scope, "MaxNumberOfFailedAttempts", "Maximum number of login failure",
				"Maximum number of attempt to login before blocking the user", ConfigType.Integer, 405, 5.ToString(),
				validationregex: @"^[0-9]+$");
			CreateConfig(scope, "NumberOfDaysToChangePassword", "Number of days before new password",
				"Number of days before the applicatoin ask for a new password", ConfigType.Integer, 406, 0.ToString(),
				validationregex: @"^[0-9]+$");

			#endregion
		}

		#region Views

		// TODO: reput alias
		protected const string ViewShareAccess = @"CREATE VIEW view_share_access AS
				                                   SELECT access_shares.sharing_user_id AS sharing_user_id,
				                                          access_shares.shared_user_id AS shared_user_id,
				                                          access_shares.id AS id,
				                                          access_shares.creation_date AS creation_date,
				                                          access_shares.last_update_date AS last_update_date,
				                                          access_shares.last_update_by AS last_update_by,
				                                          u1.first_name AS sharing_first_name,
				                                          u1.last_name AS sharing_last_name,
				                                          u1.username AS sharing_username,
				                                          u2.first_name AS shared_first_name,
				                                          u2.last_name AS shared_last_name,
				                                          u2.username AS shared_username
				                                   FROM access_shares 
				                                   INNER JOIN users u1 ON access_shares.sharing_user_id = u1.id
				                                   INNER JOIN users u2 ON access_shares.shared_user_id = u2.id
				                                   WHERE u1.is_disabled = {0}
				                                      AND u2.is_disabled = {0}";

		#endregion

		protected virtual void CreateViews(TransactionScope scope)
		{
			Logger.Info("Create views");
			scope.Connection.Execute(string.Format(ViewShareAccess, GetBoolValue(false)));
		}

		protected virtual string GetBoolValue(bool boolean)
		{
			switch (Config.DatabaseType)
			{
				case DatabaseType.PostgreSQL:
					return boolean.ToString();
				case DatabaseType.SQLite:
					return boolean ? "1" : "0";
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		protected virtual void Finish(TransactionScope scope)
		{
			Logger.Info("Finish");
		}

		protected void CreateConfig(TransactionScope scope, string key, string displayName, string description,
			ConfigType type, int order, string defaultValue, string value = null, string validationregex = null,
			bool dangerous = false)
		{
			scope.Connection.Insert(new DbConfig
			{
				Id = Guid.NewGuid(),
				Key = key,
				Value = value ?? defaultValue,
				DefaultValue = defaultValue,
				DisplayName = displayName,
				Description = description,
				ValidationRegex = validationregex ?? string.Empty,
				Type = type,
				Order = order,
				Dangerous = dangerous,
				CreationDate = DefaultDateTime,
				LastUpdateBy = DefaultUser,
				LastUpdateDate = DefaultDateTime
			});
		}
	}
}
