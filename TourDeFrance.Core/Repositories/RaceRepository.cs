using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools.Cache;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public class RaceRepository : BaseRepository, IRaceRepository
	{
		protected const string RaceObjectName = "Race";
		protected const string RaceStageObjectName = "Race - Stage";

		[Cache(ArgumentOrder = 0)]
		public DbRace GetRaceById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbRace>(id, RaceObjectName, throwIfNotExist);
		}

		public DbRace GetRaceByName(string name, bool throwIfNotExist = true)
		{
			return GetDbObjectByName<DbRace>(name, RaceObjectName, throwIfNotExist);
		}

		public IEnumerable<DbRace> GetAllRaces()
		{
			return GetAllDbObjects<DbRace>();
		}

		public DbRace CreateRace(string name)
		{
			name.EnsureIsNotEmpty("Name can't be empty");

			using (var scope = new TransactionScope())
			{
				EnsureObjectWithSameNameDoesNotExist<DbRace>(name, RaceObjectName);
				var race = new DbRace {Name = name};
				race.SetOwner();
				race.BeforeInsert();
				scope.Connection.Insert(race);
				scope.Complete();
				return race;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbRace) }, typeArgumentOrders: new[] { 0 })]
		public DbRace UpdateRace(Guid id, string name)
		{
			name.EnsureIsNotEmpty("Name can't be empty");

			using (var scope = new TransactionScope())
			{
				var race = GetRaceById(id);
				EnsureUserHasRightToManipulateObject(race, ActionType.Update, RaceObjectName);
				EnsureObjectWithSameNameDoesNotExist(name, RaceObjectName, race);
				race.Name = name;
				race.BeforeUpdate();
				scope.Connection.Update<DbRace>(race);
				scope.Complete();
				return race;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbRace) }, typeArgumentOrders: new[] { 0 })]
		public DbRace DeleteRace(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				var race = GetRaceById(id);
				EnsureUserHasRightToManipulateObject(race, ActionType.Delete, RaceObjectName);
				if (scope.Connection.Count<DbGame>(x => x.RaceId == race.Id) > 0)
				{
					throw new TourDeFranceException("Can't delete a race used in one or more games");
				}
				scope.Connection.DeleteAll<DbRaceStage>(x => x.RaceId == race.Id);
				scope.Connection.DeleteAll<DbRace>(x => x.Id == race.Id);
				scope.Complete();
				return race;
			}
		}

		public RaceStage AddStageToRace(Guid raceId, Guid stageId)
		{
			using (var scope = new TransactionScope())
			{
				var race = GetRaceById(raceId);
				EnsureUserHasRightToManipulateObject(race, ActionType.Update, RaceObjectName);
				var stage = StageRepository.GetStageById(stageId);
				if (scope.Connection.FirstOrDefault<DbRaceStage>(x => x.RaceId == race.Id && x.StageId == stage.Id) != null)
				{
					throw new TourDeFranceException("Can't add the same stage twice in a Race");
				}
				var order = (int) scope.Connection.Count<DbRaceStage>(x => x.RaceId == raceId) + 1;
				var stageRace = new DbRaceStage {Order = order, RaceId = raceId, StageId = stageId};
				stageRace.BeforeInsert();
				scope.Connection.Insert(stageRace);

				RaceStage result = GetRaceStageViewById(stageRace.Id);
				Cache.Remove(stageRace.Id.GenerateCacheKey<DbRaceStage>());

				scope.Complete();
				return result;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbRaceStage) }, typeArgumentOrders: new[] { 0 })]
		public RaceStage RemoveStageFromRace(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				RaceStage stageRace = GetRaceStageViewById(id);
				DbRace race = GetRaceById(stageRace.RaceId);
				EnsureUserHasRightToManipulateObject(race, ActionType.Update, RaceObjectName);
				scope.Connection.DeleteAll<DbRaceStage>(x => x.Id == id);
				foreach (var stage in scope.Connection.Select<DbRaceStage>(x => x.RaceId == race.Id && x.Order > stageRace.Order))
				{
					stage.Order--;
					scope.Connection.Update(stage, x => x.Order);
				}
				scope.Complete();
				return stageRace;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbRaceStage) }, typeArgumentOrders: new[] { 0 })]
		public RaceStage ChangeStageOrder(Guid id, int newOrder)
		{
			newOrder.EnsureIsStrictlyPositive("Order mus be > 0");

			using (var scope = new TransactionScope())
			{
				DbRaceStage stageRace = GetRaceStageById(id);
				DbRace race = GetRaceById(stageRace.RaceId);
				EnsureUserHasRightToManipulateObject(race, ActionType.Update, RaceStageObjectName);
				if (newOrder > GetStagesForRace(race.Id).Count())
				{
					throw new TourDeFranceException("Order of stages in race cannot exceed number of stages in this race");
				}

				if (stageRace.Order > newOrder)
				{
					foreach (var stage in scope.Connection.Select<DbRaceStage>(x => x.Order >= newOrder && x.Order < stageRace.Order))
					{
						stage.Order++;
						scope.Connection.Update(stage, x => x.Order);
						Cache.Remove(stage.Id.GenerateCacheKey<DbRaceStage>());
					}
				}
				else if (stageRace.Order < newOrder)
				{
					foreach (var stage in scope.Connection.Select<DbRaceStage>(x => x.Order > stageRace.Order && x.Order <= newOrder))
					{
						stage.Order--;
						scope.Connection.Update(stage, x => x.Order);
						Cache.Remove(stage.Id.GenerateCacheKey<DbRaceStage>());
					}
				}

				stageRace.Order = newOrder;
				stageRace.BeforeUpdate();
				scope.Connection.Update<DbRaceStage>(stageRace);

				RaceStage result = GetRaceStageViewById(stageRace.Id);
				scope.Complete();
				return result;
			}
		}

		[Cache(ArgumentOrder = 0)]
		public DbRaceStage GetRaceStageById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbRaceStage>(id, RaceStageObjectName, throwIfNotExist);
		}

		public RaceStage GetRaceStageViewById(Guid raceStageId)
		{
			using (var scope = new TransactionScope())
			{
				DbRaceStage dbRaceStage = RaceRepository.GetRaceStageById(raceStageId);
				var builder = RaceStage.Sqlbuilder
					.Where<DbRaceStage>(x => x.Id == dbRaceStage.Id);
				RaceStage raceStage = scope.Connection.Query<RaceStage>(builder.ToSql(), builder.Parameters).Single();
				scope.Complete();
				return raceStage;
			}
		}

		public IEnumerable<RaceStage> GetStagesForRace(Guid raceId)
		{
			using (var scope = new TransactionScope())
			{
				DbRace race = RaceRepository.GetRaceById(raceId);
				var builder = RaceStage.Sqlbuilder
					.Where<DbRaceStage>(x => x.RaceId == race.Id)
					.OrderBy<DbRaceStage>(x => x.Order);
				IList<RaceStage> stages = scope.Connection.Query<RaceStage>(builder.ToSql(), builder.Parameters).ToList();
				scope.Complete();
				return stages;
			}
		}
	}
}