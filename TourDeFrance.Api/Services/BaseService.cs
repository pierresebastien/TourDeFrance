using Nancy;
using ServiceStack.Logging;
using ServiceStack.ServiceInterface.ServiceModel;
using TourDeFrance.Api.Exceptions;
using TourDeFrance.Core.Exceptions;

namespace TourDeFrance.Api.Services
{
	// TODO: restricts all info returned by API base on connected users => improve information + determine some busiess rules to know accessible "objects"
	// TODO: use get text for messages returned by the api (error mainly)
	public abstract class BaseService : NancyModule
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(BaseService));

		protected BaseService(string modulePath) : base(modulePath)
		{
			OnError.AddItemToEndOfPipeline((context, exception) =>
			{
				HttpStatusCode status = HttpStatusCode.InternalServerError;

				Logger.Error($"Error while executing Service : {status}", exception);

				if (exception is NotFoundException)
				{
					status = HttpStatusCode.NotFound;
				}
				else if(exception is BadRequestException)
				{
					status = HttpStatusCode.BadRequest;
					// TODO: Retrieve Errors from the ModelValidationResult and add them in the ErrorResponse
				}
				else if (exception is TourDeFranceException)
				{
					status = HttpStatusCode.BadRequest;
				}
				return Negotiate.WithModel(new ErrorResponse(exception)).WithStatusCode(status);
			});
		}
	}
}
