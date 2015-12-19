using System.Web.Mvc;

namespace TourDeFrance.ASP.Common.Controllers
{
	[Authorize]
	public class HomeController : BaseController
	{
		public ActionResult Index()
		{
			return View();
		}
	}
}