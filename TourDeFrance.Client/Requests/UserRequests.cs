using System;
using TourDeFrance.Client.Enums;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public abstract class BaseUserRequest
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Gender Gender { get; set; }

		public string Email { get; set; }

		public DateTime? BirthDate { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }

		public bool IsAdministrator { get; set; }

		public string Password { get; set; }

		public bool RequireNewPasswordAtLogon { get; set; }
	}

	public class CreateUserRequest : BaseUserRequest
	{
		public string UserName { get; set; }

		public bool SendMail { get; set; }
	}

	public class UpdateUserRequest : BaseUserRequest, IIdentifiable<Guid>
	{
		public Guid Id { get; set; }
	}
}
