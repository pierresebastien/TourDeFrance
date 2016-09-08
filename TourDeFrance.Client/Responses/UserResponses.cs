using System;
using TourDeFrance.Client.Enums;
using TourDeFrance.Client.User;

namespace TourDeFrance.Client.Responses
{
	public class AuthenticatedUser : User
	{
		public DateTime LastPasswordChangeDate { get; set; }

		public bool RequireNewPasswordAtLogon { get; set; }
		
		public int NumberOfFailedAttempts { get; set; }

		public AccessShare[] AccessShares { get; set; }
	}

	public class User : SimpleUser
	{
		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }

		public string Email { get; set; }

		public DateTime? BirthDate { get; set; }

		public string ApiKey { get; set; }
	}

	public class SimpleUser
	{
		public Guid Id { get; set; }

		public string Username { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Gender Gender { get; set; }

		public string DisplayName { get; set; }

		public bool IsAdministrator { get; set; }

		public bool IsBlocked { get; set; }

		public bool IsDisabled { get; set; }
	}
}
