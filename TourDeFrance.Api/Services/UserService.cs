using TourDeFrance.Client.Requests;
using Nancy.ModelBinding;
using Nancy;
using TourDeFrance.Client.Responses;
using TourDeFrance.Core;

namespace TourDeFrance.Api.Services
{
	public class UserService : BaseService
	{
		public UserService() : base("/users")
		{
			Get["/me"] = _ => Negotiate.WithModel(Me());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetUser(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateUser(this.BindAndValidate<CreateUserRequest>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateUser(this.BindAndValidate<UpdateUserRequest>()));
		}

		public AuthenticatedUser Me()
		{
			// TODO: to change
			// return CurrentUser.ToAuthenticatedModel();
			return UserRepository.GetAuthenticatedUser(Constants.ADMIN_USERNAME).ToAuthenticatedModel();
		}

		public User GetUser(ObjectByGuidRequest request)
		{
			// TODO: add security checks???
			return UserRepository.GetDbUserById(request.Id).ToModel();
		}

		public User CreateUser(CreateUserRequest request)
		{
			return
				UserRepository.CreateUser(request.UserName, request.FirstName, request.LastName, request.Gender, request.Email,
					request.BirthDate, request.Height, request.Weight, request.IsAdministrator, request.Password, request.SendMail,
					request.RequireNewPasswordAtLogon).ToModel();
		}

		public User UpdateUser(UpdateUserRequest request)
		{
			return
				UserRepository.UpdateUser(request.Id, request.FirstName, request.LastName, request.Gender, request.Email,
					request.BirthDate, request.Height, request.Weight, request.IsAdministrator, request.Password,
					request.RequireNewPasswordAtLogon).ToModel();
		}
	}
}
