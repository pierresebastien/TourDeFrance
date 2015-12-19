using System.Collections.Generic;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IConfigurationRepository
	{
		DbConfig SetValue(string key, string value);

		T GetValue<T>(string key);

		DbConfig GetConfigByKey(string key, bool throwIfNotExist = true);

		IEnumerable<DbConfig> GetConfigs();
	}

	public interface IConfigurationRepositoryExtended : IConfigurationRepository
	{
		void InitValue(string key);
	}
}
