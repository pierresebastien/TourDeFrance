using System;
using TourDeFrance.Client.Requests;
using TourDeFrance.Client.User;
using Nancy.ModelBinding;
using Nancy;

namespace TourDeFrance.Api.Services
{
	public class UserService : BaseService
	{
		public UserService() : base("/users")
		{
			Get["/me"] = _ => Negotiate.WithModel(Me());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetUser(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateUser(this.BindAndValidate<>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateUser(this.BindAndValidate<>()));
		}

		public AuthenticatedUser Me()
		{
			return CurrentUser.ToAuthenticatedModel();
		}

		public User GetUser(ObjectByGuidRequest request)
		{
			// TODO: add security checks???
			return UserRepository.GetDbUserById(request.Id).ToModel();
		}

		public User CreateUser(CreateUser request)
		{
			return
				UserRepository.CreateUser(request.UserName, request.FirstName, request.LastName, request.Gender, request.Email,
					request.BirthDate, request.Height, request.Weight, request.IsAdministrator, request.Password, request.SendMail,
					request.RequireNewPasswordAtLogon).ToModel();
		}

		public User UpdateUser(Guid userId, CreateUser request)
		{
			return
				UserRepository.UpdateUser(userId, request.FirstName, request.LastName, request.Gender, request.Email,
					request.BirthDate, request.Height, request.Weight, request.IsAdministrator, request.Password,
					request.RequireNewPasswordAtLogon).ToModel();
		}
	}
}
