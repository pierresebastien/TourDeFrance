using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using TourDeFrance.Client.Drink;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Extensions;
using TourDeFrance.Core.Repositories.Interfaces;
using TourDeFrance.Core.Tools.Cache;
using TourDeFrance.Core.Tools.DataBase;
using Drink = TourDeFrance.Core.Business.Drink;
using SubDrink = TourDeFrance.Core.Business.SubDrink;

namespace TourDeFrance.Core.Repositories
{
	public class DrinkRepository : BaseRepository, IDrinkRepository
	{
		protected const string DrinkObjectName = "Drink";

		public Drink CreateDrink(string name, decimal? alcoholByVolume, decimal? volume, IEnumerable<SubDrinkDefinition> subDrinks)
		{
			name.EnsureIsNotEmpty("Name can't be empty");
			IList<SubDrinkDefinition> subDrinksList = subDrinks?.ToList() ?? new List<SubDrinkDefinition>();
			if (subDrinksList.Any())
			{
				foreach (var subDrink in subDrinksList)
				{
					subDrink.Volume.EnsureIsStrictlyPositive("Sub drink volume must be > 0");
				}
				alcoholByVolume = null;
				volume = null;
			}
			else
			{
				alcoholByVolume.EnsureIsNotNull("Alcohol by volume must be specified");
				volume.EnsureIsNotNull("Volume must be specified");
				alcoholByVolume?.EnsureIsPositive("Alcohol by volume must be >= 0");
				volume?.EnsureIsStrictlyPositive("Volume mus be > 0");
			}

			using (var scope = new TransactionScope())
			{
				EnsureObjectWithSameNameDoesNotExist<DbDrink>(name, DrinkObjectName);
				DbDrink drink = new DbDrink {Name = name, AlcoholByVolume = alcoholByVolume, Volume = volume};
				drink.SetOwner();
				drink.BeforeInsert();
				scope.Connection.Insert(drink);

				foreach (var subDrink in subDrinksList)
				{
					Drink dbDrink = GetDrinkById(subDrink.DrinkId);
					if (dbDrink.IsComposedDrink)
					{
						throw new TourDeFranceException("Can't compose a drink with sub drink composed");
					}
					DbSubDrink dbSubDrink = new DbSubDrink
					{
						DrinkId = drink.Id,
						SubDrinkId = dbDrink.Id,
						Volume = subDrink.Volume
					};
					dbSubDrink.BeforeInsert();
					scope.Connection.Insert(dbSubDrink);
				}

				Drink result = GetDrinkById(drink.Id);
				Cache.Remove(drink.Id.GenerateCacheKey<Drink>());

				scope.Complete();
				return result;
			}
		}

		[InvalidateCache(types: new[] { typeof(Drink) }, typeArgumentOrders: new[] { 0 })]
		public Drink UpdateDrink(Guid id, string name, decimal? alcoholByVolume, decimal? volume, IEnumerable<SubDrinkDefinition> subDrinks)
		{
			name.EnsureIsNotEmpty("Name can't be empty");
			IList<SubDrinkDefinition> subDrinksList = subDrinks?.ToList() ?? new List<SubDrinkDefinition>();
			if (subDrinksList.Any())
			{
				if (subDrinksList.Any(x => x.DrinkId == id))
				{
					throw new ArgumentException("Can't compose a drink with self as sub drink");
				}
				foreach (var subDrink in subDrinksList)
				{
					subDrink.Volume.EnsureIsStrictlyPositive("Sub drink volume must be > 0");
				}
				alcoholByVolume = null;
				volume = null;
			}
			else
			{
				alcoholByVolume.EnsureIsNotNull("Alcohol by volume must be specified");
				volume.EnsureIsNotNull("Volume must be specified");
				alcoholByVolume?.EnsureIsPositive("Alcohol by volume must be >= 0");
				volume?.EnsureIsStrictlyPositive("Volume mus be > 0");
			}

			using (var scope = new TransactionScope())
			{
				DbDrink drink = GetDrinkById(id);
				EnsureUserHasRightToManipulateObject(drink, ActionType.Update, DrinkObjectName);
				EnsureObjectWithSameNameDoesNotExist(name, DrinkObjectName, drink);
				drink.Name = name;
				drink.AlcoholByVolume = alcoholByVolume;
				drink.Volume = volume;
				drink.BeforeUpdate();
				scope.Connection.Update<DbDrink>(drink);

				scope.Connection.DeleteAll<DbSubDrink>(x => x.DrinkId == drink.Id);
				foreach (var subDrink in subDrinksList)
				{
					Drink dbDrink = GetDrinkById(subDrink.DrinkId);
					if (dbDrink.IsComposedDrink)
					{
						throw new TourDeFranceException("Can't compose a drink with sub drink composed");
					}
					DbSubDrink dbSubDrink = new DbSubDrink
					{
						DrinkId = drink.Id,
						SubDrinkId = dbDrink.Id,
						Volume = subDrink.Volume
					};
					dbSubDrink.BeforeInsert();
					scope.Connection.Insert(dbSubDrink);
				}

				Cache.Remove(drink.Id.GenerateCacheKey<Drink>());
				Drink result = GetDrinkById(drink.Id);
				scope.Complete();
				return result;
			}
		}

		[InvalidateCache(types: new[] { typeof(Drink) }, typeArgumentOrders: new[] { 0 })]
		public Drink DeleteDrink(Guid id)
		{
			using (var scope = new TransactionScope())
			{
				Drink drink = GetDrinkById(id);
				EnsureUserHasRightToManipulateObject(drink, ActionType.Delete, DrinkObjectName);
				if (scope.Connection.Count<DbSubDrink>(x => x.SubDrinkId == drink.Id) > 0)
				{
					throw new TourDeFranceException("Can't delete a drink used in one or more composed drinks");
				}
				if (scope.Connection.Count<DbStageDrink>(x => x.DrinkId == drink.Id) > 0)
				{
					throw new TourDeFranceException("Can't delete a drink used in one or more stages");
				}
				scope.Connection.DeleteAll<DbSubDrink>(x => x.DrinkId == drink.Id);
				scope.Connection.DeleteAll<DbDrink>(x => x.Id == drink.Id);

				scope.Complete();
				return drink;
			}
		}

		[Cache(ArgumentOrder = 0)]
		public Drink GetDrinkById(Guid id, bool throwIfNotExist = true)
		{
			using (var scope = new TransactionScope())
			{
				Drink drink = GetDbObjectById<Drink>(id, DrinkObjectName, throwIfNotExist);
				var query = SubDrink.Sqlbuilder.Where<DbSubDrink>(x => x.DrinkId == drink.Id);
				drink.SubDrinks = scope.Connection.Query<SubDrink>(query.ToSql(), query.Parameters).ToList();
				scope.Complete();
				return drink;
			}
		}

		public Drink GetDrinkByName(string name, bool throwIfNotExist = true)
		{
			using (var scope = new TransactionScope())
			{
				Drink drink = GetDbObjectByName<Drink>(name, DrinkObjectName, throwIfNotExist);
				var query = SubDrink.Sqlbuilder.Where<DbSubDrink>(x => x.DrinkId == drink.Id);
				drink.SubDrinks = scope.Connection.Query<SubDrink>(query.ToSql(), query.Parameters).ToList();
				scope.Complete();
				return drink;
			}
		}

		public IEnumerable<Drink> GetAllDrinks()
		{
			using (var scope = new TransactionScope())
			{
				IList<Drink> drinks = GetAllDbObjects<Drink>().ToList();
				IList<SubDrink> subDrinks = scope.Connection.Query<SubDrink>(SubDrink.Sqlbuilder.ToSql(), SubDrink.Sqlbuilder.Parameters).ToList();
				foreach (var drink in drinks)
				{
					drink.SubDrinks = subDrinks.Where(x => x.DrinkId == drink.Id).ToList();
				}
				scope.Complete();
				return drinks;
			}
		}
	}
}