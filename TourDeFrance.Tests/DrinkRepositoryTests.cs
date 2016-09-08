using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Tools.DataBase;
using Drink = TourDeFrance.Core.Business.Drink;
using SubDrink = TourDeFrance.Core.Business.SubDrink;

namespace TourDeFrance.Tests
{
	public abstract partial class BaseRepositoryTests
	{
		// TODO: add tests on get composed drinks + custom fields of drink (doesn't exist in dbdrink)
		#region Get drink

		[Test]
		public void GetDrinkEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.GetDrinkById(Guid.Empty));
		}

		[Test]
		public void GetDrinkNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => DrinkRepository.GetDrinkById(Guid.NewGuid()));
		}

		[Test]
		public void GetDrinkByIdOk()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 3, null);
			Assert.AreEqual("Test", drink.Name);
			Assert.AreEqual(1, drink.AlcoholByVolume);
			Assert.AreEqual(3, drink.Volume);
			Assert.AreEqual(0, drink.SubDrinks.Count);
			Assert.AreEqual(CurrentUser.Id, drink.Owner);

			Drink getDrink = DrinkRepository.GetDrinkById(drink.Id);
			Assert.AreEqual(drink.Name, getDrink.Name);
			Assert.AreEqual(drink.AlcoholByVolume, getDrink.AlcoholByVolume);
			Assert.AreEqual(drink.Volume, getDrink.Volume);
			Assert.AreEqual(drink.SubDrinks.Count, getDrink.SubDrinks.Count);
			Assert.AreEqual(drink.Owner, getDrink.Owner);
		}

		[Test]
		public void CacheOnGetDrinkById()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 3, null);
			Assert.AreEqual("Test", drink.Name);
			Assert.AreEqual(1, drink.AlcoholByVolume);
			Assert.AreEqual(3, drink.Volume);
			Assert.AreEqual(0, drink.SubDrinks.Count);
			Assert.AreEqual(CurrentUser.Id, drink.Owner);
			CheckCache(drink, drink.Id, id => DrinkRepository.GetDrinkById(id));

			drink = DrinkRepository.UpdateDrink(drink.Id, "Drink", 3, 1, null);
			Assert.AreEqual("Drink", drink.Name);
			Assert.AreEqual(3, drink.AlcoholByVolume);
			Assert.AreEqual(1, drink.Volume);
			Assert.AreEqual(0, drink.SubDrinks.Count);
			Assert.AreEqual(CurrentUser.Id, drink.Owner);
			CheckCache(drink, drink.Id, id => DrinkRepository.GetDrinkById(id));

			DrinkRepository.DeleteDrink(drink.Id);
			Assert.IsNull(GetCacheObject<Drink>(drink.Id));
			Assert.Throws<NotFoundException>(() => DrinkRepository.GetDrinkById(drink.Id));
		}

		[Test]
		public void GetDrinkEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.GetDrinkByName("   "));
		}

		[Test]
		public void GetDrinkNonExistentName()
		{
			Assert.Throws<NotFoundException>(() => DrinkRepository.GetDrinkByName("Test"));
		}

		[Test]
		public void GetDrinkByNameOk()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 3, null);
			Assert.AreEqual("Test", drink.Name);
			Assert.AreEqual(1, drink.AlcoholByVolume);
			Assert.AreEqual(3, drink.Volume);
			Assert.AreEqual(0, drink.SubDrinks.Count);
			Assert.AreEqual(CurrentUser.Id, drink.Owner);

			Drink getDrink = DrinkRepository.GetDrinkByName("Test");
			Assert.AreEqual(drink.Id, getDrink.Id);
			Assert.AreEqual(drink.AlcoholByVolume, getDrink.AlcoholByVolume);
			Assert.AreEqual(drink.Volume, getDrink.Volume);
			Assert.AreEqual(drink.SubDrinks.Count, getDrink.SubDrinks.Count);
			Assert.AreEqual(drink.Owner, getDrink.Owner);
		}

		#endregion

		#region Create drink

		[Test]
		public void CreateDrinkEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.CreateDrink("   ", 1, 1, null));
		}

		[Test]
		public void CreateDrinkNullAlcoholByVolume()
		{
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.CreateDrink("Test", null, 1, null));
		}

		[Test]
		public void CreateDrinkNegativeAlcoholByVolume()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => DrinkRepository.CreateDrink("Test", -1, 1, null));
		}

		[Test]
		public void CreateDrinkNullVolume()
		{
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.CreateDrink("Test", 1, null, null));
		}

		[Test]
		public void CreateDrinkZeroVolume()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => DrinkRepository.CreateDrink("Test", 1, 0, null));
		}

		[Test]
		public void CreateDrinkNameAlreadyExists()
		{
			DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<NameAlreadyExistsException>(() => DrinkRepository.CreateDrink("Test", 2, 2, null));
		}

		[Test]
		public void CreateDrinkZeroVolumeForSubDrink()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 2, null);
			IList<SubDrinkDefinition> subDrinks =
				new List<SubDrinkDefinition> { new SubDrinkDefinition { DrinkId = drink.Id, Volume = 0 } };
			Assert.Throws<ArgumentOutOfRangeException>(() => DrinkRepository.CreateDrink("Cocktail", null, null, subDrinks));
		}

		[Test]
		public void CreateDrinkEmptySubDrinkId()
		{
			IList<SubDrinkDefinition> subDrinks =
				new List<SubDrinkDefinition> { new SubDrinkDefinition { DrinkId = Guid.Empty, Volume = 1 } };
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.CreateDrink("Cocktail", null, null, subDrinks));
		}

		[Test]
		public void CreateDrinkNonExistentSubDrinkId()
		{
			IList<SubDrinkDefinition> subDrinks =
				new List<SubDrinkDefinition> { new SubDrinkDefinition { DrinkId = Guid.NewGuid(), Volume = 1 } };
			Assert.Throws<NotFoundException>(() => DrinkRepository.CreateDrink("Cocktail", null, null, subDrinks));
		}

		[Test]
		public void CreateDrinkSubDrinkComposed()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			SubDrinkDefinition subDrink = new SubDrinkDefinition {DrinkId = drink.Id, Volume = 1};
			Drink cocktail = DrinkRepository.CreateDrink("Cocktail", null, null, new List<SubDrinkDefinition> {subDrink});
			subDrink.DrinkId = cocktail.Id;
			Assert.Throws<TourDeFranceException>(
				() => DrinkRepository.CreateDrink("Drink", null, null, new List<SubDrinkDefinition> {subDrink}));
		}

		[Test]
		public void CreateDrinkOk()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 2, null);
			Assert.AreEqual("Test", drink.Name);
			Assert.AreEqual(1, drink.AlcoholByVolume);
			Assert.AreEqual(2, drink.Volume);
			Assert.AreEqual(CurrentUser.Id, drink.Owner);
			Assert.AreEqual(0, drink.SubDrinks.Count);

			SubDrinkDefinition subDrinkDef = new SubDrinkDefinition {DrinkId = drink.Id, Volume = 5};
			Drink cocktail = DrinkRepository.CreateDrink("Cocktail", 1, 1, new List<SubDrinkDefinition> {subDrinkDef});
			Assert.AreEqual("Cocktail", cocktail.Name);
			Assert.IsNull(cocktail.AlcoholByVolume);
			Assert.IsNull(cocktail.Volume);
			Assert.AreEqual(CurrentUser.Id, cocktail.Owner);
			Assert.AreEqual(1, cocktail.SubDrinks.Count);

			SubDrink subDrink = cocktail.SubDrinks.Single();
			Assert.AreEqual(drink.Id, subDrink.SubDrinkId);
			Assert.AreEqual(cocktail.Id, subDrink.DrinkId);
			Assert.AreEqual(5, subDrink.Volume);
		}

		#endregion

		#region Update drink

		[Test]
		public void UpdateDrinkEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.UpdateDrink(Guid.Empty, "Test", 1, 1, null));
		}

		[Test]
		public void UpdateDrinkNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => DrinkRepository.UpdateDrink(Guid.NewGuid(), "Test", 1, 1, null));
		}

		[Test]
		public void UpdateDrinkEmptyName()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.UpdateDrink(drink.Id, "   ", 1, 1, null));
		}

		[Test]
		public void UpdateDrinkNullAlcoholByVolume()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.UpdateDrink(drink.Id, "Test", null, 1, null));
		}

		[Test]
		public void UpdateDrinkNegativeAlcoholByVolume()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<ArgumentOutOfRangeException>(() => DrinkRepository.UpdateDrink(drink.Id, "Test", -1, 1, null));
		}

		[Test]
		public void UpdateDrinkNullVolume()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.UpdateDrink(drink.Id, "Test", 1, null, null));
		}

		[Test]
		public void UpdateDrinkZeroVolume()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<ArgumentOutOfRangeException>(() => DrinkRepository.UpdateDrink(drink.Id, "Test", 1, 0, null));
		}

		[Test]
		public void UpdateDrinkNameAlreadyExists()
		{
			DrinkRepository.CreateDrink("Test", 1, 1, null);
			Drink drink = DrinkRepository.CreateDrink("Drink", 2, 1, null);
			Assert.Throws<NameAlreadyExistsException>(() => DrinkRepository.UpdateDrink(drink.Id, "Test", 3, 4, null));
		}

		[Test]
		public void UpdateDrinkZeroVolumeForSubDrink()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 2, null);
			SubDrinkDefinition subDrink = new SubDrinkDefinition {DrinkId = drink.Id, Volume = 1};
			Drink cocktail = DrinkRepository.CreateDrink("Cocktail", null, null, new List<SubDrinkDefinition> {subDrink});
			subDrink.Volume = 0;
			Assert.Throws<ArgumentOutOfRangeException>(
				() => DrinkRepository.UpdateDrink(cocktail.Id, "Cocktail", null, null, new List<SubDrinkDefinition> {subDrink}));
		}

		[Test]
		public void UpdateDrinkEmptySubDrinkId()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 2, null);
			SubDrinkDefinition subDrink = new SubDrinkDefinition { DrinkId = drink.Id, Volume = 1 };
			Drink cocktail = DrinkRepository.CreateDrink("Cocktail", null, null, new List<SubDrinkDefinition> { subDrink });
			subDrink.DrinkId = Guid.Empty;
			Assert.Throws<ArgumentNullException>(
				() => DrinkRepository.UpdateDrink(cocktail.Id, "Cocktail", null, null, new List<SubDrinkDefinition> { subDrink }));
		}

		[Test]
		public void UpdateDrinkNonExistentSubDrinkId()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 2, null);
			SubDrinkDefinition subDrink = new SubDrinkDefinition { DrinkId = drink.Id, Volume = 1 };
			Drink cocktail = DrinkRepository.CreateDrink("Cocktail", null, null, new List<SubDrinkDefinition> { subDrink });
			subDrink.DrinkId = Guid.NewGuid();
			Assert.Throws<NotFoundException>(
				() => DrinkRepository.UpdateDrink(cocktail.Id, "Cocktail", null, null, new List<SubDrinkDefinition> { subDrink }));
		}

		[Test]
		public void UpdateDrinkSelfSubDrinkId()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 2, null);
			SubDrinkDefinition subDrink = new SubDrinkDefinition { DrinkId = drink.Id, Volume = 1 };
			Drink cocktail = DrinkRepository.CreateDrink("Cocktail", null, null, new List<SubDrinkDefinition> { subDrink });
			subDrink.DrinkId = cocktail.Id;
			Assert.Throws<ArgumentException>(
				() => DrinkRepository.UpdateDrink(cocktail.Id, "Cocktail", null, null, new List<SubDrinkDefinition> { subDrink }));
		}

		[Test]
		public void UpdateDrinkSubDrinkComposed()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			SubDrinkDefinition subDrink = new SubDrinkDefinition { DrinkId = drink.Id, Volume = 1 };
			Drink cocktail = DrinkRepository.CreateDrink("Cocktail", null, null, new List<SubDrinkDefinition> { subDrink });
			Drink composed = DrinkRepository.CreateDrink("Composed", null, null, new List<SubDrinkDefinition> { subDrink });
			subDrink.DrinkId = composed.Id;
			Assert.Throws<TourDeFranceException>(
				() => DrinkRepository.UpdateDrink(cocktail.Id, "Cocktail", null, null, new List<SubDrinkDefinition> { subDrink }));
		}

		[Test]
		public void UpdateDrinkNotOwner()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => DrinkRepository.UpdateDrink(drink.Id, "Test", 3, 1, null));

			AsOtherAdmin();
			drink = DrinkRepository.UpdateDrink(drink.Id, "Drink", 3, 5, null);
			Assert.AreEqual("Drink", drink.Name);
			Assert.AreEqual(3, drink.AlcoholByVolume);
			Assert.AreEqual(5, drink.Volume);
			Assert.AreEqual(0, drink.SubDrinks.Count);
			Assert.AreNotEqual(CurrentUser.Id, drink.Owner);

			AsAdmin();
			Assert.AreEqual(CurrentUser.Id, drink.Owner);
		}

		[Test]
		public void UpdateDrinkOk()
		{
			Drink subDrink = DrinkRepository.CreateDrink("SubDrink", 1, 1, null);

			Drink drink = DrinkRepository.CreateDrink("Test", 1, 2, null);
			Assert.AreEqual("Test", drink.Name);
			Assert.AreEqual(1, drink.AlcoholByVolume);
			Assert.AreEqual(2, drink.Volume);
			Assert.AreEqual(0, drink.SubDrinks.Count);
			Assert.AreEqual(CurrentUser.Id, drink.Owner);

			SubDrinkDefinition subDrinkDef = new SubDrinkDefinition { DrinkId = subDrink.Id, Volume = 8 };
			drink = DrinkRepository.UpdateDrink(drink.Id, "Drink", 5, 3, new List<SubDrinkDefinition> {subDrinkDef});
			Assert.AreEqual("Drink", drink.Name);
			Assert.IsNull(drink.AlcoholByVolume);
			Assert.IsNull(drink.Volume);
			Assert.AreEqual(1, drink.SubDrinks.Count);

			SubDrink subDrinkOfCocktail = drink.SubDrinks.Single();
			Assert.AreEqual(subDrink.Id, subDrinkOfCocktail.SubDrinkId);
			Assert.AreEqual(drink.Id, subDrinkOfCocktail.DrinkId);
			Assert.AreEqual(8, subDrinkOfCocktail.Volume);

			drink = DrinkRepository.UpdateDrink(drink.Id, "Cocktail", 7, 6, null);
			Assert.AreEqual("Cocktail", drink.Name);
			Assert.AreEqual(7, drink.AlcoholByVolume);
			Assert.AreEqual(6, drink.Volume);
			Assert.AreEqual(0, drink.SubDrinks.Count);
		}

		#endregion

		#region Delete drink

		[Test]
		public void DeleteDrinkEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => DrinkRepository.DeleteDrink(Guid.Empty));
		}

		[Test]
		public void DeleteDrinkNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => DrinkRepository.DeleteDrink(Guid.NewGuid()));
		}

		[Test]
		public void DeleteDrinkNotOwner()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => DrinkRepository.DeleteDrink(drink.Id));

			AsOtherAdmin();
			DrinkRepository.DeleteDrink(drink.Id);
		}

		[Test]
		public void DeleteDrinkUsedInStages()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 600);
			StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);
			Assert.Throws<TourDeFranceException>(() => DrinkRepository.DeleteDrink(drink.Id));
		}

		[Test]
		public void DeleteDrinkUsedInComposedDrink()
		{
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			SubDrinkDefinition subDrinkDef = new SubDrinkDefinition {DrinkId = drink.Id, Volume = 1};
			DrinkRepository.CreateDrink("Cocktail", null, null, new List<SubDrinkDefinition> {subDrinkDef});
			Assert.Throws<TourDeFranceException>(() => DrinkRepository.DeleteDrink(drink.Id));
		}

		[Test]
		public void DeleteDrinkOk()
		{
			Drink subDrink = DrinkRepository.CreateDrink("SubDrink", 1, 1, null);
			SubDrinkDefinition subDrinkDef = new SubDrinkDefinition { DrinkId = subDrink.Id, Volume = 1 };
			Drink drink = DrinkRepository.CreateDrink("Test", 1, 1, new List<SubDrinkDefinition> { subDrinkDef });
			using (var scope = new TransactionScope())
			{
				Assert.AreEqual(1, scope.Connection.Count<DbSubDrink>(x => x.DrinkId == drink.Id));
				scope.Complete();
			}

			DrinkRepository.DeleteDrink(drink.Id);
			Assert.Throws<NotFoundException>(() => DrinkRepository.GetDrinkById(drink.Id));
			using (var scope = new TransactionScope())
			{
				Assert.AreEqual(0, scope.Connection.Count<DbSubDrink>(x => x.DrinkId == drink.Id));
				scope.Complete();
			}
		}

		#endregion
	}
}
