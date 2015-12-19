using System;
using System.Linq;
using System.Text;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Interfaces;
using TourDeFrance.Core.Repositories.Interfaces;
using ServiceStack.CacheAccess;
using ServiceStack.Text;
using System.Collections.Generic;
using SimpleStack.Orm;
using SimpleStack.Orm.Expressions;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public abstract class BaseRepository
	{
		protected enum ActionType
		{
			Create,
			Update,
			Delete
		}

		protected AuthenticatedUser CurrentUser => Context.Current.User;

		protected AuthenticatedUser CurrentRealUser => Context.Current.RealUser;

		protected IDialectProvider DialectProvider => Context.Current.DialectProvider;

		protected Config Config => Context.Current.Config;

		public ICacheClient Cache => Context.Current.Cache;

		public IEmailSender EmailSender => Context.Current.EmailSender;

		#region Repositories

		public IDrinkRepository DrinkRepository => Context.Current.DrinkRepository;

		public IStageRepository StageRepository => Context.Current.StageRepository;

		public IUserRepository UserRepository => Context.Current.UserRepository;

		public IRaceRepository RaceRepository => Context.Current.RaceRepository;

		public IPlayerRepository PlayerRepository => Context.Current.PlayerRepository;

		public ITeamRepository TeamRepository => Context.Current.TeamRepository;

		public IGameRepository GameRepository => Context.Current.GameRepository;

		public IRiderRepository RiderRepository => Context.Current.RiderRepository;

		public IConfigurationRepository ConfigurationRepository => Context.Current.ConfigurationRepository;

		public IEmailTemplateRepository EmailTemplateRepository => Context.Current.EmailTemplateRepository;

		public ISearchHistoryRepository SearchHistoryRepository => Context.Current.SearchHistoryRepository;

		#endregion

		protected readonly string IndexHistorySetName;

		protected BaseRepository()
		{
			IndexHistorySetName = $"lucene:{IndexName.History}:{IndexAction.Index}".FormatQueueKey();
		}

		protected void AddLog(Guid? userId, LogType type, object parameters, Guid? gameId = null, Guid? gamParticipantId = null)
		{
			using (var scope = new TransactionScope())
			{
				var log = new DbLog
				{
					Date = DateTime.Now,
					GameId = gameId,
					UserId = userId,
					GameParticipantId = gamParticipantId,
					Type = type,
					Paramaters = parameters.ToJson() // TODO: convert anonymous object to dictionary string, string
				};
				log.BeforeInsert();
				scope.Connection.Insert(log);

				// TODO: if lucene but no redis => another system to generate pile
				if (Config.UseLucene)
				{
					scope.RedisTransaction.QueueCommand(x => x.AddItemToSortedSet(IndexHistorySetName, log.Id.ToString(), 1));
				}

				scope.Complete();
			}
		}

		protected void AddLog(LogType type, object parameters, Guid? gameId = null, Guid? gamParticipantId = null)
		{
			AddLog(CurrentUser.Id, type, parameters, gameId, gamParticipantId);
		}

		// TODO: check in context
		protected void EnsureUserParticipateInGame(Guid gameId)
		{
		}

		protected void EnsureGameIsInState(DbGame game, GameStatus status)
		{
			EnsureGameIsInState(game, new[] {status});
		}

		protected void EnsureGameIsInState(DbGame game, GameStatus[] status)
		{
			if (!status.Any(gameStatus => game.Status == gameStatus))
			{
				var error = new StringBuilder("Game status must be in state ");
				for (var i = 0; i < status.Length; i++)
				{
					if (i > 0)
					{
						if (i == status.Length)
						{
							error.Append("or ");
						}
						error.Append(", ");
					}
					error.Append(status);
				}
				throw new TourDeFranceException(error.ToString());
			}
		}

		/// <summary>
		///     Throw UnauthorizedAccessException if the Current authenticated used is not an Admin.
		/// </summary>
		protected void EnsureUserIsAdmin()
		{
			if (!CurrentUser.IsAdministrator)
			{
				throw new UnauthorizedAccessException("User is not admin");
			}
		}

		protected bool IsOwner<T>(T obj)
			where T : IOwnObject
		{
			return CurrentUser.IsAdministrator || obj.Owner == CurrentUser.Id;
		}

		protected void EnsureUserHasRightToManipulateObject<T>(T obj, ActionType type, string objectName)
			where T : IOwnObject
		{
			if (!IsOwner(obj))
			{
				throw new NotOwnerException(
					$"You can't {type.ToString().ToLower()} a {objectName.ToLower()} if you are not the owner");
			}
		}

		protected void EnsureObjectWithSameNameDoesNotExist<T>(string name, string objectName, T existingObject = null)
			where T : class, INameable, IIdentifiable<Guid>
		{
			T t = GetDbObjectByName<T>(name, objectName, false);
			if (t != null && (existingObject == null || existingObject.Id != t.Id))
			{
				throw new NameAlreadyExistsException($"A {objectName} with name '{name}' already exists");
			}
		}

		protected IEnumerable<T> GetAllDbObjects<T>()
		{
			using (var scope = new TransactionScope())
			{
				IList<T> results = scope.Connection.Select<T>().ToList();
				scope.Complete();
				return results;
			}
		}

		protected T GetDbObjectById<T>(Guid id, string objectName, bool throwIfNotExist = true)
			where T : IIdentifiable<Guid>
		{
			id.EnsureIsNotEmpty(objectName + " id can't be empty");
			using (var scope = new TransactionScope())
			{
				T result = scope.Connection.FirstOrDefault<T>(x => x.Id == id);
				if (result == null && throwIfNotExist)
				{
					throw new NotFoundException($"{objectName} with id '{id}' not found");
				}
				scope.Complete();
				return result;
			}
		}

		protected T GetDbObjectByName<T>(string name, string objectName, bool throwIfNotExist = true)
			where T : INameable
		{
			name.EnsureIsNotEmpty(objectName + " name can't be null or empty");
			using (var scope = new TransactionScope())
			{
				T result = scope.Connection.FirstOrDefault<T>(x => x.Name == name);
				if (result == null && throwIfNotExist)
				{
					throw new NotFoundException($"{objectName} with name '{name}' not found");
				}
				scope.Complete();
				return result;
			}
		}

		protected PagingResult<T> SearchDbObjects<T>(Action<SqlExpressionVisitor<T>> where, int offset, int max, Action<SqlExpressionVisitor<T>> orderBy = null)
			where T : class, new()
		{
			IList<T> results;
			int total;
			using (var scope = new TransactionScope())
			{
				SqlExpressionVisitor<T> expression = DialectProvider.ExpressionVisitor<T>();
				where(expression);
				total = (int)scope.Connection.Count(expression);

				orderBy?.Invoke(expression);
				expression.Limit(offset, max);
				results = scope.Connection.Select(expression).ToList();

				scope.Complete();
			}
			return new PagingResult<T> { Results = results, Total = total, Offset = offset, Max = max };
		}

		protected void RemoveKeysFromCache(params string[] baseKeys)
		{
			Cache.RemoveAll(baseKeys.Select(x => x.FormatCacheKey()));
		}
	}
}