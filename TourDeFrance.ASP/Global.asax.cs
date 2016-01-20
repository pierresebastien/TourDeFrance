using System;
using System.Threading;
using log4net;
using log4net.Config;
using TourDeFrance.ASP.Common.Tools;

namespace TourDeFrance.ASP
{
	public class MvcApplication : System.Web.HttpApplication
	{
		private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

		static MvcApplication()
		{
			XmlConfigurator.Configure();
		}

		protected void Application_Start()
		{
			Logger.Info("Application Starting");
		}

		protected void Session_Start(object sender, EventArgs e)
		{
			try
			{
				Logger.Debug($"Session started with id '{Session.SessionID}'");
				OwinHelper.ApplicationSetup.InitializeContext();
			}
			catch (Exception exception)
			{
				Logger.Error($"Error while initializing context for session with id '{Session.SessionID}'", exception);
			}
		}

		protected void Session_End()
		{
			Logger.Debug($"Session ended with id '{Session.SessionID}'");
		}

		protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();
			if (ex != null)
			{
				if (ex is ThreadAbortException)
				{
					return;
				}
				Logger.Error("Application Error", ex);
				Response.Redirect("~/Content/UnhandledError.html");
			}
		}

		protected void Application_End(object sender, EventArgs e)
		{
			Logger.Info("Application Ending");
		}
	}
}
