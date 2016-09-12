using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using TourDeFrance.Core.Tools.Cache;
using System.Linq.Expressions;
using ServiceStack.Text;
using System.Text.RegularExpressions;
using ServiceStack.Common;
using SimpleStack.Orm.Expressions;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Email;
using TourDeFrance.Core.Logging;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public class UserRepository : BaseRepository, IUserRepository
	{
		private static readonly ILog Logger = LogProvider.For<UserRepository>();
		protected static readonly Regex UsernameRegex = new Regex("[A-Za-z0-9._-]+", RegexOptions.Compiled);

		protected const string UserObjectName = "User"; // TODO: to use

		public DbUser CreateUser(string userName, string firstName, string lastName, Gender gender, string email,
			DateTime? birthDate, decimal? height, decimal? weight, bool isAdministrator, string password, bool sendMail,
			bool requireNewPasswordAtLogon)
		{
			EnsureUserIsAdmin();
			password.EnsureIsNotEmpty("Password can't be empty");
			password.EnsureIsSecure();
			userName.EnsureIsNotEmpty("Username can't be empty");
			userName.EnsureMatchRegex(UsernameRegex, "Username may only contain the follow characters : A-Z a-z 0-9 . - _ ");
			userName.EnsureIsAtLeastNCaracter(Config.MinUserNameLength,
				$"Username must be at least {Config.MinUserNameLength} chars");
			firstName.EnsureIsNotEmpty("First name can't be empty");
			lastName.EnsureIsNotEmpty("Last name can't be empty");
			email.EnsureIsAValidEmailAddress("Email is not a valid email address");
			birthDate?.EnsureIsInPast("Birth date must be in the past");
			height?.EnsureIsStrictlyPositive("Height must be > 0");
			weight?.EnsureIsStrictlyPositive("Weight must be > 0");

			using (var scope = new TransactionScope())
			{
				DbUser user = GetDbUserByUsername(userName, false);
				if (user != null)
				{
					throw new NameAlreadyExistsException($"A user with name '{userName}' already exists");
				}

				var saltedHash = new SaltedHash();
				string salt;
				string hash;
				saltedHash.GetHashAndSaltString(password, out hash, out salt);
				user = new DbUser
				{
					Username = userName,
					FirstName = firstName,
					LastName = lastName,
					BirthDate = birthDate,
					Email = email,
					IsAdministrator = isAdministrator,
					Password = hash,
					Salt = salt,
					LastPasswordChangeDate = DateTime.Now.ToUniversalTime(),
					RequireNewPasswordAtLogon = requireNewPasswordAtLogon,
					ApiKey = Guid.NewGuid().ToString(),
					IsBlocked = false,
					IsDisabled = false,
					NumberOfFailedAttempts = 0,
					Gender = gender,
					Height = height,
					Weight = weight
				};
				HandlePreviousPasswords(user, password, hash, salt);
				user.BeforeInsert();
				scope.Connection.Insert(user);

				AddLog(LogType.CreateUser, new {user.Username});
				AddLog(user.Id, LogType.UserCreated, new {CurrentUser.DisplayName});

				if (sendMail)
				{
					DbMailTemplate template =
						scope.Connection.Select<DbMailTemplate>(
							x => x.Type == DbMailTemplate.UserCreated && x.DefaultMailTemplateId.HasValue).SingleOrDefault();
					EmailSender.SendEmail(new[] {email}, DbMailTemplate.UserCreated, template?.Id,
						new UserCreatedEmailModel
						{
							DisplayName = user.DisplayName,
							UserName = userName,
							Password = password
						});
				}

				scope.Complete();
				return user;
			}
		}

		public DbUser UpdateUser(Guid userId, string firstName, string lastName, Gender gender, string email,
			DateTime? birthDate, decimal? height, decimal? weight, bool isAdministrator, string password, bool requireNewPasswordAtLogon)
		{
			if (!CurrentUser.IsAdministrator && userId != CurrentUser.Id)
			{
				throw new UnauthorizedAccessException();
			}

			if (!string.IsNullOrWhiteSpace(password))
			{
				password.EnsureIsNotEmpty("Password can't be empty");
				password.EnsureIsSecure();
			}
			firstName.EnsureIsNotEmpty("First name can't be empty");
			lastName.EnsureIsNotEmpty("Last name can't be empty");
			email.EnsureIsAValidEmailAddress("Email is not a valid email address");
			birthDate?.EnsureIsInPast("Birth date must be in the past");
			height?.EnsureIsStrictlyPositive("Height must be > 0");
			weight?.EnsureIsStrictlyPositive("Weight must be > 0");

			using (var scope = new TransactionScope())
			{
				DbUser user = GetDbUserById(userId);

				user.FirstName = firstName;
				user.LastName = lastName;
				user.BirthDate = birthDate;
				user.Email = email;
				user.Gender = gender;
				user.Height = height;
				user.Weight = weight;

				//Need to be administrator to change administrator rights and to change the status of the require new password
				if (CurrentUser.IsAdministrator)
				{
					user.IsAdministrator = isAdministrator;
					user.RequireNewPasswordAtLogon = requireNewPasswordAtLogon;
				}

				if (password != null)
				{
					var saltedHash = new SaltedHash();
					string salt;
					string hash;
					saltedHash.GetHashAndSaltString(password, out hash, out salt);
					HandlePreviousPasswords(user, password, hash, salt, CurrentUser.IsAdministrator);
					user.Password = hash;
					user.Salt = salt;
					user.LastPasswordChangeDate = DateTime.Now;
				}
				user.BeforeUpdate();
				AddLog(LogType.UpdateUser, new {user.Username});
				scope.Connection.Update<DbUser>(user);
				ClearUserCache(user);

				scope.Complete();
				return user;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbUser) }, typeArgumentOrders: new[] { 0 })]
		public void DisableUser(Guid id)
		{
			EnsureUserIsAdmin();

			if (id == CurrentUser.Id)
			{
				throw new TourDeFranceException("You can't disable the account you're logged on!");
			}

			using (var scope = new TransactionScope())
			{
				DbUser existing = GetDbUserById(id);
				existing.IsDisabled = true;
				scope.Connection.Update(existing, x => x.IsDisabled);
				AddLog(LogType.DisableUser, new { existing.Username });
				scope.Complete();
				ClearUserCache(existing);
			}
		}

		[InvalidateCache(types: new[] { typeof(DbUser) }, typeArgumentOrders: new[] { 0 })]
		public void EnableUser(Guid id)
		{
			EnsureUserIsAdmin();

			if (id == CurrentUser.Id)
			{
				throw new TourDeFranceException("You can't enable the account you're logged on!");
			}

			using (var scope = new TransactionScope())
			{
				DbUser existing = GetDbUserById(id);
				existing.IsDisabled = false;
				scope.Connection.Update(existing, x => x.IsDisabled);
				AddLog(LogType.EnableUser, new { existing.Username });
				scope.Complete();
			}
		}

		[InvalidateCache(types: new[] { typeof(DbUser) }, typeArgumentOrders: new[] { 0 })]
		public void UnblockUser(Guid id)
		{
			EnsureUserIsAdmin();

			using (var scope = new TransactionScope())
			{
				DbUser existing = GetDbUserById(id);
				AddLog(LogType.UnblockUser, new { existing.Username });
				existing.IsBlocked = false;
				existing.NumberOfFailedAttempts = 0;
				scope.Connection.Update(existing, x => new {x.IsBlocked, x.NumberOfFailedAttempts });
				scope.Complete();
			}
		}

		#region Authentication

		[InvalidateCache(types: new[] { typeof(AuthenticatedUser) }, typeArgumentOrders: new[] { 0 })]
		public virtual bool ValidateUser(string username, string password)
		{
			using (var scope = new TransactionScope())
			{
				var user = GetDbUserByUsername(username, false);
				if (user == null || user.IsBlocked || user.IsDisabled)
				{
					return false;
				}

				var saltedHash = new SaltedHash();
				bool passwordOk = saltedHash.VerifyHashString(password, user.Password, user.Salt);
				if (Config.MaxNumberOfFailedAttempts != 0)
				{
					if (!passwordOk)
					{
						user.NumberOfFailedAttempts += 1;
						if (user.NumberOfFailedAttempts >= Config.MaxNumberOfFailedAttempts)
						{
							AddLog(user.Id, LogType.BlockUser, new { Config.MaxNumberOfFailedAttempts });
							user.IsBlocked = true;
						}
					}
					else
					{
						user.NumberOfFailedAttempts = 0;
					}
					scope.Connection.Update(user, x => new {x.IsBlocked, x.NumberOfFailedAttempts });
				}

				scope.Complete();
				return passwordOk;
			}
		}

		[Cache(ArgumentOrder = 0)]
		public virtual AuthenticatedUser GetAuthenticatedUser(string username)
		{
			AuthenticatedUser authenticatedUser = null;
			using (var scope = new TransactionScope())
			{
				var user = GetDbUserByUsername(username, false);
				if (user != null)
				{
					authenticatedUser = user.TranslateTo<AuthenticatedUser>(); // TODO: remove translate to usage
				}
				scope.Complete();
			}
			return authenticatedUser;
		}

		#endregion

		#region Passwod Management

		public virtual string UserForgotPassword(string username, string email)
		{
			Logger.Info($"User forgot password procedure started for user '{username}' ({email})");
			DbUser user = null;
			using (var scope = new TransactionScope())
			{
				if (!string.IsNullOrWhiteSpace(username))
				{
					user = GetAuthenticatedUser(username);
				}
				else if (!string.IsNullOrWhiteSpace(email))
				{
					user = GetUserByEmailAddress(email);
				}

				if (user == null || user.IsDisabled)
				{
					throw new NotFoundException("User not found.");
				}

				// Use FormAuthentication to Encrypt/Decrypt authTicket
				DateTime expirationDate = DateTime.Now.AddHours(Config.NumberOfHourBeforeTokenExpiration);
				var ticket = new FormsAuthenticationTicket(1, user.Id.ToString(), DateTime.Now, expirationDate, false,
					user.DisplayName);
				string authToken = FormsAuthentication.Encrypt(ticket);

				DbMailTemplate template =
					scope.Connection.Select<DbMailTemplate>(
						x => x.Type == DbMailTemplate.LostPassword && x.DefaultMailTemplateId.HasValue).SingleOrDefault();

				EmailSender.SendEmail(new[] {user.Email}, DbMailTemplate.LostPassword, template?.Id,
					new LostPasswordEmailModel
					{
						UserName = user.DisplayName,
						LoginName = user.Username,
						AuthToken = authToken,
						ExpirationDate = expirationDate
					});
				scope.Complete();
			}
			return user.Email;
		}

		public virtual void ChangePassword(string oldPassword, string newPassword)
		{
			oldPassword.EnsureIsNotEmpty("Old Password must be specified");
			newPassword.EnsureIsNotEmpty("New Password must be specified");
			newPassword.EnsureIsSecure();
			if (CurrentUser == null)
			{
				throw new TourDeFranceException("You must be connected to change password");
			}

			DbUser user = GetDbUserById(CurrentUser.Id);
			var saltedHash = new SaltedHash();
			if (!saltedHash.VerifyHashString(oldPassword, user.Password, user.Salt))
			{
				throw new TourDeFranceException("Old password is incorrect");
			}

			string salt;
			string hash;
			saltedHash.GetHashAndSaltString(newPassword, out hash, out salt);

			HandlePreviousPasswords(user, newPassword, hash, salt);

			user.Password = hash;
			user.Salt = salt;
			user.RequireNewPasswordAtLogon = false;
			user.LastPasswordChangeDate = DateTime.Now;
			user.BeforeUpdate();

			using (var scope = new TransactionScope())
			{
				scope.Connection.Update<DbUser>(user);
				AddLog(user.Id, LogType.ChangePassword, new { user.Username });
				ClearUserCache(user);
				scope.Complete();
			}
		}
		
		public virtual void RecoverPassword(string authToken, string newPassword)
		{
			authToken.EnsureIsNotEmpty("AuthToken must be specified");
			newPassword.EnsureIsNotEmpty("New Password must be specified");
			newPassword.EnsureIsSecure();

			FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authToken);
			if (ticket == null || ticket.Expired)
			{
				throw new TourDeFranceException("Token has expired");
			}

			DbUser user = GetDbUserById(new Guid(ticket.Name));

			var saltedHash = new SaltedHash();
			string salt;
			string hash;
			saltedHash.GetHashAndSaltString(newPassword, out hash, out salt);
			HandlePreviousPasswords(user, newPassword, hash, salt);
			user.Password = hash;
			user.Salt = salt;
			user.LastPasswordChangeDate = DateTime.Now;
			user.BeforeUpdate();

			using (var scope = new TransactionScope())
			{
				scope.Connection.Update<DbUser>(user);
				AddLog(user.Id, LogType.RecoverPassword, new { user.Username });
				ClearUserCache(user);
				scope.Complete();
			}
		}

		protected virtual void HandlePreviousPasswords(DbUser user, string newPassword, string newHash, string newSalt, bool adminOverride = false)
		{
			if (Config.NumberOfPreviousPasswordsAuthorized != 0)
			{
				var saltedHash = new SaltedHash();

				Dictionary<int, Dictionary<string, string>> previousPasswords = new Dictionary<int, Dictionary<string, string>>();
				if (!string.IsNullOrWhiteSpace(user.PreviousPasswords))
				{
					previousPasswords = user.PreviousPasswords.FromJson<Dictionary<int, Dictionary<string, string>>>();
				}

				if (!adminOverride)
				{
					foreach (int key in previousPasswords.Keys)
					{
						foreach (string hash in previousPasswords[key].Keys)
						{
							if (saltedHash.VerifyHashString(newPassword, hash, previousPasswords[key][hash]))
							{
								throw new TourDeFranceException($"Your password can't be the same as the {Config.NumberOfPreviousPasswordsAuthorized} previous ones");
							}
						}
					}
				}

				while (previousPasswords.Count >= Config.NumberOfPreviousPasswordsAuthorized)
				{
					previousPasswords.Remove(previousPasswords.Min(x => x.Key));
				}

				int cpt = 1;
				Dictionary<int, Dictionary<string, string>> previousPasswordCopy =
					previousPasswords.OrderBy(x => x.Key).Select(x => x.Value).ToDictionary(previousPassord => cpt++);
				previousPasswordCopy.Add(cpt, new Dictionary<string, string> { { newHash, newSalt } });
				user.PreviousPasswords = previousPasswordCopy.ToJson();
			}
		}

		#endregion

		#region Get users

		[Cache(ArgumentOrder = 0)]
		public DbUser GetDbUserById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbUser>(id, UserObjectName, throwIfNotExist);
		}

		public DbUser GetDbUserByUsername(string username, bool throwIfNotExist = true)
		{
			return GetDbUserBy(username, x => (u => u.Username == x), "username", throwIfNotExist);
		}

		public DbUser GetDbUserByFirstName(string firstName, bool throwIfNotExist = true)
		{
			return GetDbUserBy(firstName, x => (u => u.FirstName == x), "first name", throwIfNotExist);
		}

		public DbUser GetDbUserByLastName(string lastName, bool throwIfNotExist = true)
		{
			return GetDbUserBy(lastName, x => (u => u.LastName == x), "last name", throwIfNotExist);
		}

		protected virtual DbUser GetDbUserBy<T>(T value, Func<T, Expression<Func<DbUser, bool>>> comparison, string comparisonName, bool throwIfNotExist = true)
		{
			using (var scope = new TransactionScope())
			{
				var user = scope.Connection.FirstOrDefault(comparison(value));
				if (user == null && throwIfNotExist)
				{
					throw new NotFoundException($"User with {comparisonName} '{value}' not found");
				}
				scope.Complete();
				return user;
			}
		}

		public DbUser GetUserByEmailAddress(string email, bool throwIfNotExist = true)
		{
			email.EnsureIsNotEmpty("Email is empty");

			DbUser result;
			using (var scope = new TransactionScope())
			{
				var users = scope.Connection.Select<DbUser>(x => x.Email == email).ToList();
				switch (users.Count)
				{
					case 0:
						if (throwIfNotExist)
						{
							throw new NotFoundException($"User with email '{email}' not found");
						}
						result = null;
						break;
					case 1:
						result = users.First();
						break;
					default:
						throw new TourDeFranceException($"There are multiple users with email address '{email}'");
				}
				scope.Complete();
			}
			return result;
		}
		
		public PagingResult<DbUser> GetUsers(string query, int offset, int max, bool? isBlocked, bool? isDisabled)
		{
			Action<SqlExpressionVisitor<DbUser>> where =
				x => {
					if (isDisabled.HasValue)
					{
						x.Where(y => y.IsDisabled == isDisabled.Value);
					}
					if (isBlocked.HasValue)
					{
						x.Where(y => y.IsBlocked == isBlocked.Value);
					}
					if (!string.IsNullOrWhiteSpace(query))
					{
						x.Where(
								y =>
								y.Username.Contains(query) || y.Email.Contains(query) ||
								y.FirstName.Contains(query) || y.LastName.Contains(query));
					}
				};
			Action<SqlExpressionVisitor<DbUser>> orderBy = x => x.OrderBy(y => y.Username);
			return SearchDbObjects(where, offset, max, orderBy);
		}

		#endregion

		public void ClearUserCache(DbUser user)
		{
			RemoveKeysFromCache($"{typeof (DbUser).Name}:{user.Id}", $"{typeof (AuthenticatedUser).Name}:{user.Username}");
		}
	}
}