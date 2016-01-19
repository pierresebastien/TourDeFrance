using System;
using log4net;

namespace TourDeFrance.Core.Tools
{
	public class ErrorLogger
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(ErrorLogger));

		// TODO: to improve
		public static void LogException(Exception e, string url = "", string message = "Exception : ")
		{
			ThreadContext.Properties["Url"] = url;
			ThreadContext.Properties["User"] = Context.Current.User?.Username;
			Logger.Error(message, e);
		}
	}
}
