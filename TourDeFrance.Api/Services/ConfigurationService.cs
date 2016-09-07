using Nancy;
using Nancy.ModelBinding;
using System.Linq;
using TourDeFrance.Client.Config;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Services
{
	public class ConfigurationService : BaseService
	{
		public ConfigurationService() : base("/configs")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllConfigs());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetConfig(this.BindAndValidate<ObjectByIdRequest>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateConfig(this.BindAndValidate<UpdateConfigRequest>()));
		}

		public Config[] GetAllConfigs()
		{
			return ConfigurationRepository.GetConfigs().Select(x => x.ToModel()).ToArray();
		}

		public Config GetConfig(ObjectByIdRequest request)
		{
			return ConfigurationRepository.GetConfigByKey(request.Id).ToModel();
		}

		public Config UpdateConfig(UpdateConfigRequest request)
		{
			return ConfigurationRepository.SetValue(request.Id, request.Value).ToModel();
		}
	}
}
