using System;
using System.Web.Mvc;
using System.Web.Routing;
using TourDeFrance.ASP.Common.Tools;
using TourDeFrance.Core;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Logging;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;

namespace TourDeFrance.ASP.Common.Controllers
{
	[HandleError]
	public abstract class BaseController : Controller
	{
		private static readonly ILog Logger = LogProvider.For<BaseController>();

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

		protected override void OnException(ExceptionContext filterContext)
		{
			if (filterContext != null)
			{
				string url = filterContext.HttpContext.Request.Url.ToString();
				ErrorLogger.LogException(filterContext.Exception, url);
				// TODO: does not exists
				filterContext.Result = RedirectToAction("E500", "Error");
				filterContext.ExceptionHandled = true;
			}
			base.OnException(filterContext);
		}

		protected override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);
			if (!Context.IsContextInitialized)
			{
				Logger.Warn("Context not initialized!");
				if (System.Web.HttpContext.Current == null)
				{
					Logger.Warn("HttpContext.Current is null");
				}
				else if (System.Web.HttpContext.Current.Session == null)
				{
					Logger.Warn("HttpContext.Current.Session is null");
				}

				if (filterContext.HttpContext == null)
				{
					Logger.Warn("filterContext.HttpContext is null");
				}
				else if (filterContext.HttpContext.Application == null)
				{
					Logger.Warn("filterContext.HttpContext.Application is null");
				}
				else
				{
					Logger.Warn("Initializing context to avoid further errors");
					OwinHelper.ApplicationSetup.InitializeContext();
				}
			}

			if (filterContext.HttpContext.User.Identity.IsAuthenticated)
			{
				if (Context.Current.User == null)
				{
					string[] data = filterContext.HttpContext.User.Identity.Name.Split('@');
					AuthenticatedUser user = Context.Current.UserRepository.GetAuthenticatedUser(data[0]);
					if (user == null) //if the account is still logged in but the user doesn't exists
					{
						Logger.Info("User reported as authenticated but user not found, logging off");
						Authentication.SignOut();
						filterContext.Result =
							new RedirectToRouteResult(new RouteValueDictionary(new { controller = "User", action = "Login" }));
					}
					else
					{
						Context.Current.User = user;
						if (data.Length > 1)
						{
							Logger.Info("Impersonification found, trying to impersonate");
							AuthenticatedUser impersonificationUser = Context.Current.UserRepository.GetAuthenticatedUser(data[1]);
							Context.Current.UserRepository.ConnectAs(impersonificationUser.Id);
						}
					}
				}
				else
				{
					//refresh
					Context.Current.User = Context.Current.UserRepository.GetAuthenticatedUser(Context.Current.User.Username);
				}

				if (Context.Current.User != null)
				{
					if (Context.Current.User.IsBlocked || Context.Current.User.IsDisabled)
					{
						Logger.InfoFormat("User {0}, logging off", Context.Current.User.IsDisabled ? "disabled" : "blocked");
						Authentication.SignOut();
						filterContext.Result =
							new RedirectToRouteResult(new RouteValueDictionary(new { controller = "GlobalUser", action = "Login" }));
					}
					else if (Context.Current.RealUser == null &&
							(!filterContext.HttpContext.Request.Path.Contains("/GlobalUser/ChangePassword") &&
							 !filterContext.HttpContext.Request.Path.Contains("/GlobalUser/Logoff")))
					{
						if (Context.Current.User.RequireNewPasswordAtLogon)
						{
							Logger.Info("User is required to change his password, redirect to change password page");
							filterContext.Result =
								new RedirectToRouteResult(new RouteValueDictionary(new { controller = "GlobalUser", action = "ChangePassword" }));
						}
						else if (Context.Current.Config.NumberOfDaysToChangePassword > 0 &&
								(DateTime.Now - Context.Current.User.LastPasswordChangeDate).Days >=
								Context.Current.Config.NumberOfDaysToChangePassword)
						{
							Logger.Info("User password has expired, redirect to change password page");
							filterContext.Result =
								new RedirectToRouteResult(
									new RouteValueDictionary(new { controller = "GlobalUser", action = "ChangePassword", expired = true }));
						}
					}
				}
			}
		}

		protected void AddError(string aMessage, params object[] p)
		{
			ViewBag.Error = GetFormattedMessage(aMessage, p);
		}

		protected void AddSuccess(string aMessage, params object[] p)
		{
			ViewBag.Success = GetFormattedMessage(aMessage, p);
		}

		protected void AddInfo(string aMessage, params object[] p)
		{
			ViewBag.Info = GetFormattedMessage(aMessage, p);
		}

		protected void AddWarning(string aMessage, params object[] p)
		{
			ViewBag.Warning = GetFormattedMessage(aMessage, p);
		}

		protected RedirectToRouteResult RedirectSuccess(string action, string controller, object routeValues, string message, params object[] p)
		{
			return Redirect(action, controller, routeValues, "Success", message, p);
		}

		protected RedirectToRouteResult RedirectError(string action, string controller, object routeValues, string message, params object[] p)
		{
			return Redirect(action, controller, routeValues, "Error", message, p);
		}

		protected RedirectToRouteResult RedirectInfo(string action, string controller, object routeValues, string message, params object[] p)
		{
			return Redirect(action, controller, routeValues, "Info", message, p);
		}

		protected RedirectToRouteResult RedirectWarning(string action, string controller, object routeValues, string message, params object[] p)
		{
			return Redirect(action, controller, routeValues, "Warning", message, p);
		}

		protected virtual RedirectToRouteResult Redirect(string action, string controller, object routeValues,
														 string redirectType, string message, params object[] p)
		{
			return Redirect(action, controller, new RouteValueDictionary(routeValues), redirectType, message, p);
		}

		protected virtual RedirectToRouteResult Redirect(string action, string controller, RouteValueDictionary routeValues,
														 string redirectType, string message, params object[] p)
		{
			TempData[redirectType] = GetFormattedMessage(message, p);
			return RedirectToAction(action, controller, routeValues);
		}

		protected RedirectResult RedirectSuccess(string refUrl, string message, params object[] p)
		{
			return Redirect(refUrl, "Success", message, p);
		}

		protected RedirectResult RedirectError(string refUrl, string message, params object[] p)
		{
			return Redirect(refUrl, "Error", message, p);
		}

		protected virtual RedirectResult Redirect(string refUrl, string redirectType, string message, params object[] p)
		{
			TempData[redirectType] = GetFormattedMessage(message, p);
			return Redirect(refUrl);
		}

		protected string GetFormattedMessage(string message, params object[] p)
		{
			return HtmlUtility.SanitizeHtml(string.Format(message, p).Replace("\n", "<br/>"));
		}
	}
}