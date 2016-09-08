using System;
using Nancy;
using TourDeFrance.Api.Exceptions;
using TourDeFrance.Core;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;

namespace TourDeFrance.Api.Services
{
	// TODO: investigate better solution than using context.current
	// TODO: restricts all info returned by API base on connected users => improve information + determine some busiess rules to know accessible "objects"
	public abstract class BaseService : NancyModule
	{
		protected AuthenticatedUser CurrentUser => Core.Context.Current.User;

		protected Config Config => Core.Context.Current.Config;

		public IEmailSender EmailSender => Core.Context.Current.EmailSender;

		#region Repositories

		public IDrinkRepository DrinkRepository => Core.Context.Current.DrinkRepository;

		public IStageRepository StageRepository => Core.Context.Current.StageRepository;

		public IUserRepository UserRepository => Core.Context.Current.UserRepository;

		public IRaceRepository RaceRepository => Core.Context.Current.RaceRepository;

		public IPlayerRepository PlayerRepository => Core.Context.Current.PlayerRepository;

		public ITeamRepository TeamRepository => Core.Context.Current.TeamRepository;

		public IGameRepository GameRepository => Core.Context.Current.GameRepository;

		public IRiderRepository RiderRepository => Core.Context.Current.RiderRepository;

		public IConfigurationRepository ConfigurationRepository => Core.Context.Current.ConfigurationRepository;

		public IEmailTemplateRepository EmailTemplateRepository => Core.Context.Current.EmailTemplateRepository;

		public ISearchHistoryRepository SearchHistoryRepository => Core.Context.Current.SearchHistoryRepository;

		#endregion

		protected BaseService(string modulePath) : base(modulePath)
		{
			OnError.AddItemToEndOfPipeline(ManageException);
		}

		// TODO: use get text for messages returned by the api (error mainly)
		protected dynamic ManageException(NancyContext context, Exception exception)
		{
			HttpStatusCode status;
			string content = exception.Message;

			if (exception is NotFoundException)
			{
				status = HttpStatusCode.NotFound;
			}
			else if (exception is UnauthorizedAccessException)
			{
				status = HttpStatusCode.Forbidden;
			}
			else if (exception is BadRequestException)
			{
				status = HttpStatusCode.BadRequest;
				// TODO: Retrieve Errors from the ModelValidationResult and add them in the ErrorResponse
			}
			else if (exception is TourDeFranceException)
			{
				status = HttpStatusCode.BadRequest;
			}
			else
			{
				status = HttpStatusCode.InternalServerError;
				content = string.Empty;
				ErrorLogger.LogException(exception, context.Request.Url);
			}
			// TODO: use model ???
			return Negotiate.WithModel(content).WithStatusCode(status);
		}
	}
}
