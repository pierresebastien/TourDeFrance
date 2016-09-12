using System;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IUserRepository
	{
		DbUser CreateUser(string userName, string firstName, string lastName, Gender gender, string email, DateTime? birthDate,
			decimal? height, decimal? weight, bool isAdministrator, string password, bool sendMail,
			bool requireNewPasswordAtLogon);

		DbUser UpdateUser(Guid userId, string firstName, string lastName, Gender gender, string email, DateTime? birthDate,
			decimal? height, decimal? weight, bool isAdministrator, string password, bool requireNewPasswordAtLogon);

		/// <summary>
		/// Disable a user
		/// </summary>
		/// <param name="id">id of an existing user to disable</param>
		void DisableUser(Guid id);

		void EnableUser(Guid id);

		/// <summary>
		/// Unblock a user
		/// </summary>
		/// <param name="id">id of an existing user to unblock</param>
		void UnblockUser(Guid id);

		/// <summary>
		/// Check a User password
		/// </summary>
		/// <param name="username">Username</param>
		/// <param name="password">Password</param>
		/// <returns>True if password is correct, false otherwize</returns>
		bool ValidateUser(string username, string password);

		/// <summary>
		///    Return a user by username
		/// </summary>
		/// <param name="username">Username</param>
		/// <returns></returns>
		AuthenticatedUser GetAuthenticatedUser(string username);


		/// <summary>
		/// Send an email to the user with information to recover it's password
		/// </summary>
		/// <param name="username">username</param>
		/// <param name="email">email address</param>
		/// <returns></returns>
		string UserForgotPassword(string username, string email);

		void ChangePassword(string oldPassword, string newPassword);

		/// <summary>
		///    Change the password of a user using the forgot password procedure
		/// </summary>
		/// <param name="authToken">Authentication token send by email</param>
		/// <param name="newPassword">New password</param>
		void RecoverPassword(string authToken, string newPassword);

		DbUser GetDbUserById(Guid id, bool throwIfNotExist = true);

		DbUser GetDbUserByUsername(string username, bool throwIfNotExist = true);

		DbUser GetDbUserByFirstName(string firstName, bool throwIfNotExist = true);

		DbUser GetDbUserByLastName(string lastName, bool throwIfNotExist = true);

		DbUser GetUserByEmailAddress(string email, bool throwIfNotExist = true);

		PagingResult<DbUser> GetUsers(string query, int offset, int max, bool? isBlocked, bool? isDisabled);
	}
}