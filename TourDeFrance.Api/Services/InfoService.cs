using Nancy;
namespace TourDeFrance.Api.Services
{
	public class InfoService : BaseService
	{
		public InfoService() : base("/infos")
		{
			Get["/version"] = _ => Negotiate.WithModel(new InfoResponse { Message = config.Version });
		}
	}
}
