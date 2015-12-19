using System;
using System.Configuration;
using System.Web;
using System.Web.Security;

namespace TourDeFrance.ASP.Common.Tools
{
	public class Authentication
	{
		public static FormsAuthenticationTicket GetAuthenticationTicket()
		{
			HttpContext current = HttpContext.Current;
			HttpCookie authCookie = current.Request.Cookies[FormsAuthentication.FormsCookieName];
			if (authCookie == null)
			{
				throw new HttpException("No authentification cookie found.");
			}
			return FormsAuthentication.Decrypt(authCookie.Value);
		}

		public static void SignIn(string id, bool persistant, TimeSpan duration)
		{
			SignIn(id, persistant, DateTime.Now.Add(duration));
		}

		public static void SignIn(string id, bool persistant, DateTime expiration)
		{
			HttpContext context = HttpContext.Current;
			FormsAuthenticationTicket authTicket =
				new FormsAuthenticationTicket(1, id, DateTime.Now, expiration, persistant, string.Empty);
			string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
			HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
			{
				Secure = FormsAuthentication.RequireSSL,
				HttpOnly = true,
				Path = FormsAuthentication.FormsCookiePath
			};
			if (persistant)
			{
				authCookie.Expires = authTicket.Expiration;
			}
			context.Response.Cookies.Add(authCookie);
		}

		public static void SignOut()
		{
			FormsAuthentication.SignOut();
		}

		public static TimeSpan GetTimeout(bool persistant = false)
		{
			return
				TimeSpan.FromMinutes(persistant
										 ? int.Parse(ConfigurationManager.AppSettings["loginTimeoutRememberMe"] ?? "2880")
										 : int.Parse(ConfigurationManager.AppSettings["loginTimeout"] ?? "5"));
		}
	}
}