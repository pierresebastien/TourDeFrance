using System;
using System.Web.Mvc;
using System.Web.Security;
using hbehr.recaptcha;
using TourDeFrance.ASP.Common.Models;
using TourDeFrance.ASP.Common.Tools;
using TourDeFrance.Core.Exceptions;

namespace TourDeFrance.ASP.Common.Controllers
{
	public class UserController : BaseController
	{
		public ActionResult Login()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Login(LoginModel loginModel)
		{
			if (ModelState.IsValid)
			{
				try
				{
					if (Membership.ValidateUser(loginModel.UserName, loginModel.Password))
					{
						Authentication.SignIn(loginModel.UserName, loginModel.RememberMe, Authentication.GetTimeout(loginModel.RememberMe));
						return Redirect(loginModel.Continue);
					}

					AddError("Incorrect username and/or password");
					return View();
				}
				catch (TourDeFranceException e)
				{
					AddError(e.Message);
				}
			}
			return View(loginModel);
		}

		public ActionResult ForgotPassword()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ForgotPassword(ForgotPasswordModel userForgotPasswordModel)
		{
			string userResponse = HttpContext.Request.Params["g-recaptcha-response"];
			bool validCaptcha = ReCaptcha.ValidateCaptcha(userResponse);
			if (!validCaptcha)
			{
				AddError("You are a robot");
			}
			else if (string.IsNullOrWhiteSpace(userForgotPasswordModel.Email)
			    && string.IsNullOrWhiteSpace(userForgotPasswordModel.Username))
			{
				ModelState.AddModelError(string.Empty, "Username and email are empty. Please fill at least one.");
			}
			else
			{
				try
				{
					string email = UserRepository.UserForgotPassword(userForgotPasswordModel.Username, userForgotPasswordModel.Email);
					string message = $@"We've sent an email to {email} containing a temporary url that will allow you to reset your password for the next {Config.NumberOfHourBeforeTokenExpiration} hours.
						                Please check your spam folder if the email doesn't appear within a few minutes.";
					return RedirectInfo("Login", "User", null, message);
				}
				catch (NotFoundException)
				{
					AddError("Unable to send a password reset request : User not found.");
				}
				catch (TourDeFranceException e)
				{
					AddError("Unable to send a password reset request : " + e.Message);
				}
			}
			return View();
		}

		public ActionResult RecoverPassword(string authToken)
		{
			return View(new PasswordRecoveryModel { AuthToken = authToken });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult RecoverPassword(PasswordRecoveryModel model)
		{
			if (ModelState.IsValid)
			{
				try
				{
					UserRepository.RecoverPassword(model.AuthToken, model.Password);
					return RedirectSuccess("Login", "User", null, "Password has been updated");
				}
				catch (TourDeFranceException e)
				{
					return RedirectError("Login", "User", null, e.Message);
				}
			}
			return View(model);
		}

		[Authorize]
		public ActionResult ChangePassword(bool expired = false)
		{
			return View(new ChangePasswordModel { IsExpired = expired });
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult ChangePassword(ChangePasswordModel changePasswordModel)
		{
			if (ModelState.IsValid)
			{
				try
				{
					UserRepository.ChangePassword(changePasswordModel.OldPassword, changePasswordModel.NewPassword);
					return RedirectSuccess("Index", "GlobalHome", null, "Password successfully changed");
				}
				catch (TourDeFranceException e)
				{
					ModelState.AddModelError(string.Empty, e.Message);
				}
				catch (ArgumentException e)
				{
					ModelState.AddModelError(string.Empty, e.Message);
				}
			}
			return View(changePasswordModel);
		}
	}
}
