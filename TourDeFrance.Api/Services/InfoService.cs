using Nancy;
using TourDeFrance.Client.Responses;

namespace TourDeFrance.Api.Services
{
	public class InfoService : BaseService
	{
		public InfoService() : base("/infos")
		{
			Get["/version"] = _ => Negotiate.WithModel(new Info {Message = Config.Version.ToString(3)});
		}
	}
}
