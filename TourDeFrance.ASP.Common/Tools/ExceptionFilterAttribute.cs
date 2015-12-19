using log4net;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using TourDeFrance.Core;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Business.Email;
using TourDeFrance.Core.Exceptions;

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

				try
				{
					Context.Current.EmailSender.SendEmail(new[] { Context.Current.Config.ErrorMailRecipient }, DbMailTemplate.ErrorOccurred, null,
						new ErrorEmailModel
						{
							Exception = new ExceptionEmailModel(context.Exception),
							Url = context.Request.RequestUri.AbsoluteUri
						});
				}
				catch (Exception e)
				{
					Logger.Error("Error while sending email to administrator", e);
				}
			}
			context.Response = new HttpResponseMessage(status) { Content = content };
		}
	}
}