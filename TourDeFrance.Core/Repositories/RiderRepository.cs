using System;
using System.Collections.Generic;
using System.Linq;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using TourDeFrance.Core.Tools.Cache;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	// TODO: check on nationality? on picture?
	public class RiderRepository : BaseRepository, IRiderRepository
	{
		protected const string RiderObjectName = "Rider";

		[Cache(ArgumentOrder = 0)]
		public DbRider GetRiderById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbRider>(id, RiderObjectName, throwIfNotExist);
		}

		public IEnumerable<DbRider> GetAllRiders()
		{
			return GetAllDbObjects<DbRider>();
		}

		public IEnumerable<DbRider> GetRidersForTeam(Guid teamId)
		{
			using (var scope = new TransactionScope())
			{
				var team = TeamRepository.GetTeamById(teamId);
				IList<DbRider> riders = scope.Connection.Select<DbRider>(x => x.TeamId == team.Id).ToList();
				scope.Complete();
				return riders;
			}
		}

		public DbRider CreateRider(string firstName, string lastName, Gender gender, DateTime? birthDate, string nationality, decimal? height,
			decimal? weight, byte[] picture, Guid teamId)
		{
			firstName.EnsureIsNotEmpty("First name can't be empty");
			lastName.EnsureIsNotEmpty("Last name can't be empty");
			birthDate?.EnsureIsInPast("Birth date must be in the past");
			height?.EnsureIsStrictlyPositive("Height must be > 0");
			weight?.EnsureIsStrictlyPositive("Weight must be > 0");

			using (var scope = new TransactionScope())
			{
				DbTeam team = TeamRepository.GetTeamById(teamId, false);
				if (team == null)
				{
					throw new TourDeFranceException($"Team with id '{teamId}' not found");
				}

				DbRider rider = new DbRider
				{
					FirstName = firstName,
					LastName = lastName,
					BirthDate = birthDate,
					Nationality = nationality,
					Height = height,
					Weight = weight,
					Picture = picture,
					TeamId = teamId,
					Gender = gender
				};
				rider.SetOwner();
				rider.BeforeInsert();
				scope.Connection.Insert(rider);
				scope.Complete();
				return rider;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbRider) }, typeArgumentOrders: new[] { 0 })]
		public DbRider UpdateRider(Guid id, string firstName, string lastName, Gender gender, DateTime? birthDate, string nationality,
			decimal? height, decimal? weight, byte[] picture, Guid teamId)
		{
			firstName.EnsureIsNotEmpty("First name can't be empty");
			lastName.EnsureIsNotEmpty("Last name can't be empty");
			birthDate?.EnsureIsInPast("Birth date must be in the past");
			height?.EnsureIsStrictlyPositive("Height must be > 0");
			weight?.EnsureIsStrictlyPositive("Weight must be > 0");

			using (var scope = new TransactionScope())
			{
				DbTeam team = TeamRepository.GetTeamById(teamId, false);
				if (team == null)
				{
					throw new TourDeFranceException($"Team with id '{teamId}' not found");
				}

				DbRider rider = GetRiderById(id);
				EnsureUserHasRightToManipulateObject(rider, ActionType.Update, RiderObjectName);

				rider.FirstName = firstName;
				rider.LastName = lastName;
				rider.BirthDate = birthDate;
				rider.Nationality = nationality;
				rider.Height = height;
				rider.Weight = weight;
				rider.Picture = picture;
				rider.TeamId = teamId;
				rider.Gender = gender;

				rider.BeforeUpdate();
				scope.Connection.Update<DbRider>(rider);
				scope.Complete();
				return rider;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbRider) }, typeArgumentOrders: new[] { 0 })]
		public DbRider DeleteRider(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				DbRider rider = GetRiderById(id);
				if (scope.Connection.Count<DbGameParticipant>(x => x.RiderId == rider.Id) > 0)
				{
					throw new TourDeFranceException("Can't delete a rider used in one or more games");
				}
				EnsureUserHasRightToManipulateObject(rider, ActionType.Delete, RiderObjectName);
				scope.Connection.DeleteAll<DbRider>(x => x.Id == rider.Id);
				scope.Complete();
				return rider;
			}
		}
	}
}
