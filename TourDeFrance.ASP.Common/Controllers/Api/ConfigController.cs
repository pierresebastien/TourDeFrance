using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TourDeFrance.Client.Config;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	// TODO: get config value ??
	public class ConfigController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/config")]
		public IEnumerable<Config> GetAllConfigs()
		{
			return ConfigurationRepository.GetConfigs().Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/config/{configKey}")]
		public Config GetConfig(string configKey){
			return ConfigurationRepository.GetConfigByKey(configKey).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/config/{configKey}")]
		public Config UpdateConfig(string configKey, UpdateConfig model)
		{
			return ConfigurationRepository.SetValue(configKey, model.Value).ToModel();
		}

		#endregion
	}
}
