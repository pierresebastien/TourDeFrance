using System;
using System.Collections.Generic;
using System.Web.Http;
using TourDeFrance.Client.User;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	// TODO: lot of methods missing
	public class UserController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/users/me")]
		public AuthenticatedUser Me()
		{
			return CurrentUser.ToAuthenticatedModel();
		}

		// TODO:
		//[HttpGet]
		//[Route("api/users")]
		//public IEnumerable<SimpleUser> GetUsers()
		//{
		//	return UserRepository.GetUsers();
		//}

		[HttpGet]
		[Route("api/users/{userId}")]
		public User GetUser(Guid userId)
		{
			// TODO: add security checks???
			return UserRepository.GetDbUserById(userId).ToModel();
		}

		#endregion

		#region POST

		[HttpPost]
		[Route("api/users")]
		public User CreateUser(CreateUser model)
		{
			return
				UserRepository.CreateUser(model.UserName, model.FirstName, model.LastName, model.Gender, model.Email,
					model.BirthDate, model.Height, model.Weight, model.IsAdministrator, model.Password, model.SendMail,
					model.RequireNewPasswordAtLogon).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/users/{userId}")]
		public User UpdateUser(Guid userId, CreateUser model)
		{
			return
				UserRepository.UpdateUser(userId, model.FirstName, model.LastName, model.Gender, model.Email, model.BirthDate,
					model.Height, model.Weight, model.IsAdministrator, model.Password, model.RequireNewPasswordAtLogon).ToModel();
		}

		#endregion

		#region DELETE



		#endregion
	}
}
