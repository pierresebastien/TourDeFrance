using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools;
using TourDeFrance.Core.Tools.Cache;
using TourDeFrance.Core.Tools.DataBase;

namespace TourDeFrance.Core.Repositories
{
	public class StageRepository : BaseRepository, IStageRepository
	{
		protected const string StageObjectName = "Stage";
		protected const string StageDrinkObjectName = "Stage - Drink";

		[Cache(ArgumentOrder = 0)]
		public DbStage GetStageById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbStage>(id, StageObjectName, throwIfNotExist);
		}

		public DbStage GetStageByName(string name, bool throwIfNotExist = true)
		{
			return GetDbObjectByName<DbStage>(name, StageObjectName, throwIfNotExist);
		}

		public IEnumerable<DbStage> GetAllStages()
		{
			return GetAllDbObjects<DbStage>();
		} 

		public DbStage CreateStage(string name, int timeInSeconds)
		{
			name.EnsureIsNotEmpty("Name can't be empty");
			timeInSeconds.EnsureIsStrictlyPositive("Time must be > 0");

			using (var scope = new TransactionScope())
			{
				EnsureObjectWithSameNameDoesNotExist<DbStage>(name, StageObjectName);
				DbStage stage = new DbStage { Name = name, Duration = timeInSeconds };
				stage.SetOwner();
				stage.BeforeInsert();
				scope.Connection.Insert(stage);
				scope.Complete();
				return stage;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbStage) }, typeArgumentOrders: new[] { 0 })]
		public DbStage UpdateStage(Guid id, string name, int timeInSeconds)
		{
			name.EnsureIsNotEmpty("Name can't be empty");
			timeInSeconds.EnsureIsStrictlyPositive("Time must be > 0");

			using (var scope = new TransactionScope())
			{
				DbStage stage = GetStageById(id);
				EnsureUserHasRightToManipulateObject(stage, ActionType.Update, StageObjectName);
				EnsureObjectWithSameNameDoesNotExist(name, StageObjectName, stage);
				stage.Name = name;
				stage.Duration = timeInSeconds;
				stage.BeforeUpdate();
				scope.Connection.Update<DbStage>(stage);
				scope.Complete();
				return stage;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbStage) }, typeArgumentOrders: new[] { 0 })]
		public DbStage DeleteStage(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				DbStage stage = GetStageById(id);
				EnsureUserHasRightToManipulateObject(stage, ActionType.Delete, StageObjectName);
				if (scope.Connection.Count<DbRaceStage>(x => x.StageId == stage.Id) > 0)
				{
					throw new TourDeFranceException("Can't delete an stage used in one or more race");
				}
				scope.Connection.DeleteAll<DbStageDrink>(x => x.StageId == stage.Id);
				scope.Connection.DeleteAll<DbStage>(x => x.Id == stage.Id);
				scope.Complete();
				return stage;
			}
		}

		public StageDrink AddDrinkToStage(Guid stageId, Guid drinkId, int numberToDrink, decimal? overridedVolume, StageType type)
		{
			numberToDrink.EnsureIsStrictlyPositive("Number to drink must be > 0");
			overridedVolume?.EnsureIsStrictlyPositive("Volume must be > 0");

			using (var scope = new TransactionScope())
			{
				DbStage stage = GetStageById(stageId);
				EnsureUserHasRightToManipulateObject(stage, ActionType.Update, StageObjectName);
				DbDrink drink = DrinkRepository.GetDrinkById(drinkId);

				int order = (int) scope.Connection.Count<DbStageDrink>(x => x.StageId == stageId) + 1;
				DbStageDrink stageDrink = new DbStageDrink
				{
					StageId = stage.Id,
					DrinkId = drink.Id,
					Order = order,
					OverridedVolume = overridedVolume,
					NumberToDrink = numberToDrink,
					Type = type
				};
				stageDrink.BeforeInsert();
				scope.Connection.Insert(stageDrink);

				StageDrink result = GetStageDrinkViewById(stageDrink.Id);
				Cache.Remove(stageDrink.Id.GenerateCacheKey<DbStageDrink>());

				scope.Complete();
				return result;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbStageDrink) }, typeArgumentOrders: new[] { 0 })]
		public StageDrink UpdateStageDrink(Guid id, int numberToDrink, decimal? overridedVolume, StageType type)
		{
			numberToDrink.EnsureIsStrictlyPositive("Number to drink must be > 0");
			overridedVolume?.EnsureIsStrictlyPositive("Volume must be > 0");

			using (var scope = new TransactionScope())
			{
				DbStageDrink stageDrink = GetStageDrinkById(id);
				DbStage stage = GetStageById(stageDrink.StageId);
				EnsureUserHasRightToManipulateObject(stage, ActionType.Update, StageObjectName);

				stageDrink.NumberToDrink = numberToDrink;
				stageDrink.OverridedVolume = overridedVolume;
				stageDrink.Type = type;
				stageDrink.BeforeUpdate();
				scope.Connection.Update<DbStageDrink>(stageDrink);

				StageDrink result = GetStageDrinkViewById(stageDrink.Id);
				scope.Complete();
				return result;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbStageDrink) }, typeArgumentOrders: new[] { 0 })]
		public StageDrink RemoveDrinkFromStage(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				StageDrink stageDrink = GetStageDrinkViewById(id);
				DbStage stage = GetStageById(stageDrink.StageId);
				EnsureUserHasRightToManipulateObject(stage, ActionType.Update, StageObjectName);

				scope.Connection.DeleteAll<DbStageDrink>(x => x.Id == id);
				foreach (
					var drink in scope.Connection.Select<DbStageDrink>(x => x.StageId == stage.Id && x.Order > stageDrink.Order))
				{
					drink.Order--;
					scope.Connection.Update(drink, x => x.Order);
				}
				scope.Complete();
				return stageDrink;
			}
		}

		[InvalidateCache(types: new[] { typeof(DbStageDrink) }, typeArgumentOrders: new[] { 0 })]
		public StageDrink ChangeDrinkOrder(Guid id, int newOrder)
		{
			newOrder.EnsureIsStrictlyPositive("Order mus be > 0");

			using (var scope = new TransactionScope())
			{
				DbStageDrink stageDrink = GetStageDrinkById(id);
				DbStage stage = GetStageById(stageDrink.StageId);
				EnsureUserHasRightToManipulateObject(stage, ActionType.Update, StageObjectName);
				if (newOrder > GetDrinksForStage(stage.Id).Count())
				{
					throw new TourDeFranceException("Order of drink in stage cannot exceed number of drinks in this stage");
				}

				if (stageDrink.Order > newOrder)
				{
					foreach (var drink in scope.Connection.Select<DbStageDrink>(x => x.Order >= newOrder && x.Order < stageDrink.Order)
						)
					{
						drink.Order++;
						scope.Connection.Update(drink, x => x.Order);
						Cache.Remove(drink.Id.GenerateCacheKey<DbStageDrink>());
					}
				}
				else if (stageDrink.Order < newOrder)
				{
					foreach (var drink in scope.Connection.Select<DbStageDrink>(x => x.Order > stageDrink.Order && x.Order <= newOrder)
						)
					{
						drink.Order--;
						scope.Connection.Update(drink, x => x.Order);
						Cache.Remove(drink.Id.GenerateCacheKey<DbStageDrink>());
					}
				}

				stageDrink.Order = newOrder;
				stageDrink.BeforeUpdate();
				scope.Connection.Update<DbStageDrink>(stageDrink);

				StageDrink result = GetStageDrinkViewById(stageDrink.Id);
				scope.Complete();
				return result;
			}
		}

		[Cache(ArgumentOrder = 0)]
		public DbStageDrink GetStageDrinkById(Guid id, bool throwIfNotExist = true)
		{
			return GetDbObjectById<DbStageDrink>(id, StageDrinkObjectName, throwIfNotExist);
		}

		public StageDrink GetStageDrinkViewById(Guid stageDrinkId)
		{
			using (var scope = new TransactionScope())
			{
				var dbStageDrink = StageRepository.GetStageDrinkById(stageDrinkId);
				var builder = StageDrink.Sqlbuilder
					.Where<DbStageDrink>(x => x.Id == dbStageDrink.Id);
				var stageDrink = scope.Connection.Query<StageDrink>(builder.ToSql(), builder.Parameters).Single();

				var query = SubDrink.Sqlbuilder.Where<DbSubDrink>(x => x.DrinkId == stageDrink.Id);
				IList<SubDrink> subDrinks = scope.Connection.Query<SubDrink>(query.ToSql(), query.Parameters).ToList();
				if (subDrinks.Any())
				{
					stageDrink.IsComposedDrink = true;
					stageDrink.Volume = stageDrink.Volume.CalculateDrinkVolume(subDrinks);
					stageDrink.AlcoholByVolume = stageDrink.AlcoholByVolume.CalculateDrinkAlcoholByVolume(subDrinks);
				}

				scope.Complete();
				return stageDrink;
			}
		}

		public IEnumerable<StageDrink> GetDrinksForStage(Guid stageId)
		{
			using (var scope = new TransactionScope())
			{
				var stage = StageRepository.GetStageById(stageId);
				var builder = StageDrink.Sqlbuilder
					.Where<DbStageDrink>(x => x.StageId == stage.Id)
					.OrderBy<DbStageDrink>(x => x.Order);
				IList<StageDrink> drinks = scope.Connection.Query<StageDrink>(builder.ToSql(), builder.Parameters).ToList();

				IList<SubDrink> allSubDrinks = scope.Connection.Query<SubDrink>(SubDrink.Sqlbuilder.ToSql(), SubDrink.Sqlbuilder.Parameters).ToList();
				foreach (var drink in drinks)
				{
					IList<SubDrink> subDrinks = allSubDrinks.Where(x => x.DrinkId == drink.Id).ToList();
					if (subDrinks.Any())
					{
						drink.IsComposedDrink = true;
						drink.Volume = drink.Volume.CalculateDrinkVolume(subDrinks);
						drink.AlcoholByVolume = drink.AlcoholByVolume.CalculateDrinkAlcoholByVolume(subDrinks);
					}
				}

				scope.Complete();
				return drinks;
			}
		}
	}
}