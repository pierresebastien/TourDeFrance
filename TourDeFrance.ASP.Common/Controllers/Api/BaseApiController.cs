using System.Web.Http;
using TourDeFrance.ASP.Common.Tools;
using TourDeFrance.Core;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Repositories.Interfaces;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	// TODO: authentication
	// TODO: check route prefix usage
	// TODO: methodes to get only own objects??
	// TODO: activate paging for get all methods ?
	// TODO: check if swagger exist
	[ExceptionFilter]
	public abstract class BaseApiController : ApiController
	{
		protected AuthenticatedUser CurrentUser => Context.Current.User;

		protected Config Config => Context.Current.Config;

		public IEmailSender EmailSender => Context.Current.EmailSender;

		#region Repositories

		public IDrinkRepository DrinkRepository => Context.Current.DrinkRepository;

		public IStageRepository StageRepository => Context.Current.StageRepository;

		public IUserRepository UserRepository => Context.Current.UserRepository;

		public IRaceRepository RaceRepository => Context.Current.RaceRepository;

		public IPlayerRepository PlayerRepository => Context.Current.PlayerRepository;

		public ITeamRepository TeamRepository => Context.Current.TeamRepository;

		public IGameRepository GameRepository => Context.Current.GameRepository;

		public IRiderRepository RiderRepository => Context.Current.RiderRepository;

		public IConfigurationRepository ConfigurationRepository => Context.Current.ConfigurationRepository;

		public IEmailTemplateRepository EmailTemplateRepository => Context.Current.EmailTemplateRepository;

		public ISearchHistoryRepository SearchHistoryRepository => Context.Current.SearchHistoryRepository;

		#endregion
	}
}
