using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Tools;

namespace TourDeFrance.ASP.Common.Tools
{
	public class ExceptionFilterAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ExceptionFilterAttribute));

		// TODO: argument, null & out of range exception
		public override void OnException(HttpActionExecutedContext context)
		{
			HttpContent content = new StringContent(context.Exception.Message);
			HttpStatusCode status;
			if (context.Exception is OperationCanceledException)
			{
				content = new StringContent(string.Empty);
				status = HttpStatusCode.NoContent;
			}
			else if (context.Exception is NotFoundException)
			{
				status = HttpStatusCode.NotFound;
			}
			else if (context.Exception is UnauthorizedAccessException)
			{
				status = HttpStatusCode.Forbidden;
			}
			else if (context.Exception is TourDeFranceException)
			{
				status = HttpStatusCode.BadRequest;
			}
			else
			{
				status = HttpStatusCode.InternalServerError;
				content = new StringContent(string.Empty); // TODO: remove exception message ???
				ErrorLogger.LogException(context.Exception, context.Request.RequestUri.AbsoluteUri);
			}
			context.Response = new HttpResponseMessage(status) { Content = content };
		}
	}
}