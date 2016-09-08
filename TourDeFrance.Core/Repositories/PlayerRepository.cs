using System;
using System.Collections.Generic;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools.Cache;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	// TODO: check all methods in repositories and ensure checks are made on all mandatory fields + ensure same checks in api validators
	public class PlayerRepository : BaseRepository, IPlayerRepository
	{
		protected const string PlayerObjectName = "Player";

		[Cache(ArgumentOrder = 0)]
		public DbPlayer GetPlayerById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbPlayer>(id, PlayerObjectName, throwIfNotExist);
		}

		public DbPlayer GetPlayerByNickname(string nickname, bool throwIfNotExist = true)
		{
			nickname.EnsureIsNotEmpty($"{PlayerObjectName} nickname can't be null or empty");

			using (var scope = new TransactionScope())
			{
				var player = scope.Connection.FirstOrDefault<DbPlayer>(x => x.Nickname == nickname);
				if (player == null && throwIfNotExist)
				{
					throw new NotFoundException($"{PlayerObjectName} with nickname '{nickname}' not found");
				}
				scope.Complete();
				return player;
			}
		}

		public DbPlayer GetPlayerByName(string firstName, string lastName, bool throwIfNotExist = true)
		{
			firstName.EnsureIsNotEmpty($"{PlayerObjectName} first name can't be null or empty");
			lastName.EnsureIsNotEmpty($"{PlayerObjectName} last name can't be null or empty");

			using (var scope = new TransactionScope())
			{
				var player = scope.Connection.FirstOrDefault<DbPlayer>(x => x.FirstName == firstName && x.LastName == lastName);
				if (player == null && throwIfNotExist)
				{
					throw new NotFoundException($"{PlayerObjectName} with first name '{firstName}' and last name '{lastName}' not found");
				}
				scope.Complete();
				return player;
			}
		}

		public IEnumerable<DbPlayer> GetAllPlayers()
		{
			return GetAllDbObjects<DbPlayer>();
		} 

		public DbPlayer CreatePlayer(string nickname, string firstName, string lastName, Gender gender, DateTime? birthDate, decimal? height, decimal? weight)
		{
			height?.EnsureIsStrictlyPositive("Height must be > 0");
			weight?.EnsureIsStrictlyPositive("Weight must be > 0");

			using (var scope = new TransactionScope())
			{
				DbPlayer playerWithSameName = GetPlayerByNickname(nickname, false);
				if (playerWithSameName != null)
				{
					throw new NameAlreadyExistsException($"{PlayerObjectName} with nickname '{nickname}' already exists");
				}
				playerWithSameName = GetPlayerByName(firstName, lastName, false);
				if (playerWithSameName != null)
				{
					throw new NameAlreadyExistsException($"{PlayerObjectName} with first name '{firstName}' and last name '{lastName}' already exists");
				}
				DbPlayer player = new DbPlayer
				{
					FirstName = firstName,
					LastName = lastName,
					BirthDate = birthDate,
					Nickname = nickname,
					Height = height,
					Weight = weight,
					Gender = gender
				};
				player.SetOwner();
				player.BeforeInsert();
				scope.Connection.Insert(player);
				scope.Complete();
				return player;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbPlayer) }, typeArgumentOrders: new[] { 0 })]
		public DbPlayer UpdatePlayer(Guid id, string nickname, string firstName, string lastName, Gender gender, DateTime? birthDate, decimal? height, decimal? weight)
		{
			height?.EnsureIsStrictlyPositive("Height must be > 0");
			weight?.EnsureIsStrictlyPositive("Weight must be > 0");

			using (var scope = new TransactionScope())
			{
				var player = GetPlayerById(id);
				EnsureUserHasRightToManipulateObject(player, ActionType.Update, PlayerObjectName);
				DbPlayer playerWithSameName = GetPlayerByNickname(nickname, false);
				if (playerWithSameName != null && playerWithSameName.Id != player.Id)
				{
					throw new NameAlreadyExistsException($"{PlayerObjectName} with nickname '{nickname}' already exists");
				}
				playerWithSameName = GetPlayerByName(firstName, lastName);
				if (playerWithSameName != null && playerWithSameName.Id != player.Id)
				{
					throw new NameAlreadyExistsException($"{PlayerObjectName} with first name '{firstName}' and last name '{lastName}' already exists");
				}

				player.Nickname = nickname;
				player.FirstName = firstName;
				player.LastName = lastName;
				player.Gender = gender;
				player.BirthDate = birthDate;
				player.Height = height;
				player.Weight = weight;
				player.BeforeUpdate();
				scope.Connection.Update<DbPlayer>(player);

				scope.Complete();
				return player;
			}
		}

		// TODO: à revoir avec systeme de lien user - player dans une game
		[InvalidateCache(types: new[] { typeof(DbPlayer) }, typeArgumentOrders: new[] { 0 })]
		public DbPlayer DeletePlayer(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				var player = GetPlayerById(id);
				EnsureUserHasRightToManipulateObject(player, ActionType.Delete, PlayerObjectName);
				scope.Connection.DeleteAll<DbPlayer>(x => x.Id == id);
				scope.Complete();
				return player;
			}
		}
	}
}