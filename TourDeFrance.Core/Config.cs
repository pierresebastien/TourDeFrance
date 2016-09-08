using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core
{
	public class ApplicationConfig
	{
		private const string KeyPrefix = "tourDeFrance";

		public string ConnectionString { get; set; }
		public DatabaseType DatabaseType { get; set; }
		public string RedisHost { get; set; }
		public bool UseLucene { get; set; }
		public int ApiPort { get; set; }
		public bool DisableTraceInApi { get; set; }

		public string ApplicationPath => Path.GetDirectoryName(
			Assembly.GetExecutingAssembly()
				.CodeBase.Substring(Environment.OSVersion.Platform == PlatformID.Unix ? 7 : 8));

		public static ApplicationConfig FromFile()
		{
			return new ApplicationConfig
			{
				ConnectionString = GetConfigurationValue("connectionString"),
				DatabaseType = (DatabaseType) Enum.Parse(typeof(DatabaseType), GetConfigurationValue("databaseType", "PostgreSQL")),
				RedisHost = GetConfigurationValue("redisHost"),
				UseLucene = GetConfigurationValue("useLucene", false),
				ApiPort = GetConfigurationValue("api:port", 0),
				DisableTraceInApi = GetConfigurationValue("api:disableErrorTraces", true)
			};
		}

		public static string GetConfigurationValue(string name, string defaultValue = null)
		{
			string value = GetStringConfigurationValue(name);
			return string.IsNullOrWhiteSpace(value) ? defaultValue : Environment.ExpandEnvironmentVariables(value);
		}

		public static T GetConfigurationValue<T>(string name, T defaultValue)
		{
			string value = GetStringConfigurationValue(name);
			return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Parse<T>();
		}

		private static string GetStringConfigurationValue(string key)
		{
			return GetStringConfigurationValueFromFile(KeyPrefix + ":" + key) ?? GetStringConfigurationValueFromFile(key) ?? string.Empty;
		}

		private static string GetStringConfigurationValueFromFile(string key)
		{
			string value = ConfigurationManager.AppSettings[key];
			return string.IsNullOrWhiteSpace(value) ? null : value;
		}
	}

	// TODO: change properties
	public class Config : IInitializable
	{
		private readonly IConfigurationRepositoryExtended _configurationRepository;
		private readonly ApplicationConfig _applicationConfig;

		public Config(IConfigurationRepositoryExtended configurationRepository, ApplicationConfig config)
		{
			_configurationRepository = configurationRepository;
			_applicationConfig = config;
		}

		public int Order => 0;

		public void Initialize()
		{
			Version = GetType().Assembly.GetName().Version;
			_configurationRepository.InitValue("TempFolder");
			_configurationRepository.InitValue("IndexFolder");
		}

		public Version Version { get; private set; }

		// application config (file)
		public string ApplicationPath => _applicationConfig.ApplicationPath;

		public bool UseLucene => _applicationConfig.UseLucene;

		public DatabaseType DatabaseType => _applicationConfig.DatabaseType;

		public int ApiPort => _applicationConfig.ApiPort;

		public bool DisableTraceInApi => _applicationConfig.DisableTraceInApi;

		public string ApiUri
		{
			get
			{
				if (ApiPort > 0)
				{
					return $"http{(IsSecure ? "s" : string.Empty)}://+{ApiPort}";
				}
				return string.Empty;
			}
		}

		// general
		public bool IsSecure => PublicUri.StartsWith("https");

		public string PublicUri => _configurationRepository.GetValue<string>("PublicUri");

		// format
		public string DecimalFormat => _configurationRepository.GetValue<string>("DecimalFormat");

		public CultureInfo CultureInfo => new CultureInfo(_configurationRepository.GetValue<string>("CultureInfo"));

		// folders
		public string TempFolder => _configurationRepository.GetValue<string>("TempFolder");

		public string IndexFolder => _configurationRepository.GetValue<string>("IndexFolder");

		// mail
		public string SmtpSenderEmailAddress => _configurationRepository.GetValue<string>("SmtpSenderEmailAddress");

		public string SmtpSenderDisplayName => _configurationRepository.GetValue<string>("SmtpSenderDisplayName");

		public string ErrorMailRecipient => _configurationRepository.GetValue<string>("ErrorMailRecipient");

		// user et password
		public int MinUserNameLength => _configurationRepository.GetValue<int>("MinUserNameLength");

		public int NumberOfHourBeforeTokenExpiration => _configurationRepository.GetValue<int>("NumberOfHourBeforeTokenExpiration");

		public int NumberOfPreviousPasswordsAuthorized => _configurationRepository.GetValue<int>("NumberOfPreviousPasswordsAuthorized");

		public string[] PasswordRegexes => _configurationRepository.GetValue<string[]>("PasswordRegexes");

		public string PasswordErrorMessage => _configurationRepository.GetValue<string>("PasswordErrorMessage");

		public int MaxNumberOfFailedAttempts => _configurationRepository.GetValue<int>("MaxNumberOfFailedAttempts");

		public int NumberOfDaysToChangePassword => _configurationRepository.GetValue<int>("NumberOfDaysToChangePassword");
	}
}