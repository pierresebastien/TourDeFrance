using System;
using DotLiquid;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Business.Email
{
	public class ErrorEmailModel : BaseEmailModel
	{
		public ErrorEmailModel()
		{
			Date = DateTime.Now;
			User = Core.Context.Current.User;
		}

		public DateTime Date { get; private set; }

		public ExceptionEmailModel Exception { get; set; }

		public DbUser User { get; private set; }

		public string Url { get; set; }
	}

	public class ExceptionEmailModel : Drop
	{
		private readonly Exception _exception;

		public ExceptionEmailModel(Exception exception)
		{
			_exception = exception;
		}

		public string Type => _exception.GetType().ToString();

		public string Message => _exception.Message;

		public string StackTrace => _exception.StackTrace;
	}
}
