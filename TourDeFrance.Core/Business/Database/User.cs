using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("users")]
	public class DbUser : Auditable
	{
		[Index(Unique = true)]
		[Alias("username")]
		public string Username { get; set; }

		[Alias("password")]
		public string Password { get; set; }

		[Alias("salt")]
		public string Salt { get; set; }

		[Alias("first_name")]
		public string FirstName { get; set; }

		[Alias("last_name")]
		public string LastName { get; set; }

		[Alias("gender")]
		public Gender Gender { get; set; }

		[Alias("birth_date")]
		public DateTime? BirthDate { get; set; }

		[Alias("height")]
		public decimal? Height { get; set; }

		[Alias("weight")]
		public decimal? Weight { get; set; }

		[Alias("email")]
		public string Email { get; set; }

		[Alias("is_administrator")]
		public bool IsAdministrator { get; set; }

		[Alias("is_blocked")]
		public bool IsBlocked { get; set; }

		[Alias("is_disabled")]
		public bool IsDisabled { get; set; }

		[Alias("last_password_change_date")]
		public DateTime LastPasswordChangeDate { get; set; }

		[Alias("previous_passwords")]
		public string PreviousPasswords { get; set; }

		[Alias("require_new_password_at_logon")]
		public bool RequireNewPasswordAtLogon { get; set; }

		[Alias("number_of_failed_attempts")]
		public int NumberOfFailedAttempts { get; set; }

		// TODO; useful?
		[Alias("api_key")]
		public string ApiKey { get; set; }

		[Ignore]
		public string DisplayName => FirstName + " " + LastName;

		protected T ToSimpleModel<T>() where T: Client.User.SimpleUser, new()
		{
			return new T
			{
				Id = Id,
				Gender = Gender,
				FirstName = FirstName,
				LastName = LastName,
				DisplayName = DisplayName,
				IsAdministrator = IsAdministrator,
				IsBlocked = IsBlocked,
				IsDisabled = IsDisabled,
				Username = Username
			};
		}

		public Client.User.SimpleUser ToSimpleModel()
		{
			return ToSimpleModel<Client.User.SimpleUser>();
		}

		protected T ToModel<T>() where T : Client.User.User, new()
		{
			T user = ToSimpleModel<T>();
			user.Height = Height;
			user.Weight = Weight;
			user.Email = Email;
			user.BirthDate = BirthDate;
			user.ApiKey = ApiKey;
			return user;
		}

		public Client.User.User ToModel()
		{
			return ToModel<Client.User.User>();
		}
	}
}