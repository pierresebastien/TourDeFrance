using System;
using System.Collections.Generic;
using System.Linq;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using TourDeFrance.Core.Tools.Cache;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public class TeamRepository : BaseRepository, ITeamRepository
	{
		protected const string TeamObjectName = "Team";

		[Cache(ArgumentOrder = 0)]
		public DbTeam GetTeamById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbTeam>(id, TeamObjectName, throwIfNotExist);
		}

		public DbTeam GetTeamByName(string name, bool throwIfNotExist = true)
		{
			return GetDbObjectByName<DbTeam>(name, TeamObjectName, throwIfNotExist);
		}

		public IEnumerable<DbTeam> GetAllTeams()
		{
			return GetAllDbObjects<DbTeam>();
		}

		public DbTeam CreateTeam(string name)
		{
			name.EnsureIsNotEmpty("Name can't be empty");

			using (var scope = new TransactionScope())
			{
				EnsureObjectWithSameNameDoesNotExist<DbTeam>(name, TeamObjectName);
				var team = new DbTeam {Name = name};
				team.SetOwner();
				team.BeforeInsert();
				scope.Connection.Insert(team);
				scope.Complete();
				return team;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbTeam) }, typeArgumentOrders: new[] { 0 })]
		public DbTeam UpdateTeam(Guid id, string name)
		{
			name.EnsureIsNotEmpty("Name can't be empty");

			using (var scope = new TransactionScope())
			{
				var team = GetTeamById(id);
				EnsureUserHasRightToManipulateObject(team, ActionType.Update, TeamObjectName);
				EnsureObjectWithSameNameDoesNotExist(name, TeamObjectName, team);
				team.Name = name;
				team.BeforeUpdate();
				scope.Connection.Update<DbTeam>(team);
				scope.Complete();
				return team;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbTeam) }, typeArgumentOrders: new[] { 0 })]
		public DbTeam DeleteTeam(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				var team = GetTeamById(id);
				if (RiderRepository.GetRidersForTeam(team.Id).Any())
				{
					throw new TourDeFranceException("Cannot delete a team used for one or more riders");
				}
				if (scope.Connection.Count<DbGameParticipant>(x => x.TeamId == team.Id) > 0)
				{
					throw new TourDeFranceException("Can't delete a team used in one or more games");
				}
				EnsureUserHasRightToManipulateObject(team, ActionType.Delete, TeamObjectName);
				scope.Connection.DeleteAll<DbTeam>(x => x.Id == team.Id);
				scope.Complete();
				return team;
			}
		}
	}
}