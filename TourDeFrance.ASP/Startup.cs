using Owin;
using TourDeFrance.ASP.Common.Tools;
using TourDeFrance.Core;

namespace TourDeFrance.ASP
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ApplicationConfig config = ApplicationConfig.FromFile();
			OwinHelper helper = new OwinHelper(app);
			helper.Inititialize(config);
			helper.RegisterAuthentication();
			helper.RegisterMvcApplication();
			helper.RegisterWebApiApplication();
		}
	}
}