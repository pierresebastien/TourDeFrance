using System;
using NUnit.Framework;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;
using TourDeFrance.Core.Business;

namespace TourDeFrance.Tests
{
	public abstract partial class BaseRepositoryTests
	{
		#region Get stage

		[Test]
		public void GetStageEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.GetStageById(Guid.Empty));
		}

		[Test]
		public void GetStageNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageById(Guid.NewGuid()));
		}

		[Test]
		public void GetStageByIdOk()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.AreEqual("Test", stage.Name);
			Assert.AreEqual(1, stage.Duration);
			Assert.AreEqual(CurrentUser.Id, stage.Owner);

			DbStage getStage = StageRepository.GetStageById(stage.Id);
			Assert.AreEqual(stage.Name, getStage.Name);
			Assert.AreEqual(stage.Duration, getStage.Duration);
			Assert.AreEqual(stage.Owner, getStage.Owner);
		}

		[Test]
		public void CacheOnGetStageById()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.AreEqual("Test", stage.Name);
			Assert.AreEqual(1, stage.Duration);
			Assert.AreEqual(CurrentUser.Id, stage.Owner);
			CheckCache(stage, stage.Id, id => StageRepository.GetStageById(id));

			stage = StageRepository.UpdateStage(stage.Id, "Stage", 3);
			Assert.AreEqual("Stage", stage.Name);
			Assert.AreEqual(3, stage.Duration);
			Assert.AreEqual(CurrentUser.Id, stage.Owner);
			CheckCache(stage, stage.Id, id => StageRepository.GetStageById(id));

			StageRepository.DeleteStage(stage.Id);
			Assert.IsNull(GetCacheObject<DbStage>(stage.Id));
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageById(stage.Id));
		}

		[Test]
		public void GetStageEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.GetStageByName("   "));
		}

		[Test]
		public void GetStageNonExistentName()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageByName("Test"));
		}

		[Test]
		public void GetStageByNameOk()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.AreEqual("Test", stage.Name);
			Assert.AreEqual(1, stage.Duration);
			Assert.AreEqual(CurrentUser.Id, stage.Owner);

			DbStage getStage = StageRepository.GetStageByName("Test");
			Assert.AreEqual(stage.Id, getStage.Id);
			Assert.AreEqual(stage.Duration, getStage.Duration);
			Assert.AreEqual(stage.Owner, getStage.Owner);
		}

		#endregion

		#region Create stage

		[Test]
		public void CreateStageEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.CreateStage("   ", 1));
		}

		[Test]
		public void CreateStageZeroDuration()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => StageRepository.CreateStage("Test", 0));
		}

		[Test]
		public void CreateStageNameAlreadyExists()
		{
			StageRepository.CreateStage("Test", 1);
			Assert.Throws<NameAlreadyExistsException>(() => StageRepository.CreateStage("Test", 2));
		}

		[Test]
		public void CreateStageOk()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.AreEqual("Test", stage.Name);
			Assert.AreEqual(1, stage.Duration);
			Assert.AreEqual(CurrentUser.Id, stage.Owner);
		}

		#endregion

		#region Update stage

		[Test]
		public void UpdateStageEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.UpdateStage(Guid.Empty, "Test", 1));
		}

		[Test]
		public void UpdateStageNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.UpdateStage(Guid.NewGuid(), "Test", 1));
		}

		[Test]
		public void UpdateStageEmptyName()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<ArgumentNullException>(() => StageRepository.UpdateStage(stage.Id, "   ", 1));
		}

		[Test]
		public void UpdateStageZeroRatio()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<ArgumentOutOfRangeException>(() => StageRepository.UpdateStage(stage.Id, "Test", 0));
		}

		[Test]
		public void UpdateStageNameAlreadyExists()
		{
			StageRepository.CreateStage("Test", 1);
			DbStage stage = StageRepository.CreateStage("Stage", 2);
			Assert.Throws<NameAlreadyExistsException>(() => StageRepository.UpdateStage(stage.Id, "Test", 3));
		}

		[Test]
		public void UpdateStageNotOwner()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => StageRepository.UpdateStage(stage.Id, "Test", 3));

			AsOtherAdmin();
			stage = StageRepository.UpdateStage(stage.Id, "Stage", 3);
			Assert.AreEqual("Stage", stage.Name);
			Assert.AreEqual(3, stage.Duration);
			Assert.AreNotEqual(CurrentUser.Id, stage.Owner);

			AsAdmin();
			Assert.AreEqual(CurrentUser.Id, stage.Owner);
		}

		[Test]
		public void UpdateStageOk()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.AreEqual("Test", stage.Name);
			Assert.AreEqual(1, stage.Duration);
			Assert.AreEqual(CurrentUser.Id, stage.Owner);

			stage = StageRepository.UpdateStage(stage.Id, "Stage", 3);
			Assert.AreEqual("Stage", stage.Name);
			Assert.AreEqual(3, stage.Duration);
		}

		#endregion

		#region Delete stage

		[Test]
		public void DeleteStageEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.DeleteStage(Guid.Empty));
		}

		[Test]
		public void DeleteStageNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.DeleteStage(Guid.NewGuid()));
		}

		[Test]
		public void DeleteStageNotOwner()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => StageRepository.DeleteStage(stage.Id));

			AsOtherAdmin();
			StageRepository.DeleteStage(stage.Id);
		}

		[Test]
		public void DeleteStageUsedInRaces()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			DbRace race = RaceRepository.CreateRace("Test");
			RaceRepository.AddStageToRace(race.Id, stage.Id);
			Assert.Throws<TourDeFranceException>(() => StageRepository.DeleteStage(stage.Id));
		}

		[Test]
		public void DeleteStageOk()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageRepository.DeleteStage(stage.Id);
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageById(stage.Id));
		}

		#endregion

		#region Add drink to stage

		[Test]
		public void AddDrinkToStageEmptyStageId()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<ArgumentNullException>(() => StageRepository.AddDrinkToStage(Guid.Empty, drink.Id, 1, null, StageType.Flat));
		}

		[Test]
		public void AddDrinkToStageEmptyDrinkId()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<ArgumentNullException>(() => StageRepository.AddDrinkToStage(stage.Id, Guid.Empty, 1, null, StageType.Flat));
		}

		[Test]
		public void AddDrinkToStageNonExistentStageId()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			Assert.Throws<NotFoundException>(() => StageRepository.AddDrinkToStage(Guid.NewGuid(), drink.Id, 1, null, StageType.Flat));
		}

		[Test]
		public void AddDrinkToStageNonExistentDrinkId()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<NotFoundException>(() => StageRepository.AddDrinkToStage(stage.Id, Guid.NewGuid(), 1, null, StageType.Flat));
		}

		[Test]
		public void AddDrinkToStageZeroNumber()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<ArgumentOutOfRangeException>(() => StageRepository.AddDrinkToStage(stage.Id, drink.Id, 0, null, StageType.Flat));
		}

		[Test]
		public void AddDrinkToStageZeroRatio()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<ArgumentOutOfRangeException>(() => StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, 0, StageType.Flat));
		}

		[Test]
		public void AddDrinkToStageNotOwner()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat));

			AsOtherAdmin();
			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);
			Assert.AreEqual(stage.Id, stageDrink.StageId);
			Assert.AreEqual(drink.Id, stageDrink.DrinkId);
			Assert.AreEqual(1, stageDrink.NumberToDrink);
			Assert.AreEqual(1, stageDrink.Order);
			Assert.IsNull(stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.Flat, stageDrink.Type);
		}

		[Test]
		public void AddDrinkToStageOk()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);

			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);
			Assert.AreEqual(stage.Id, stageDrink.StageId);
			Assert.AreEqual(drink.Id, stageDrink.DrinkId);
			Assert.AreEqual(1, stageDrink.NumberToDrink);
			Assert.AreEqual(1, stageDrink.Order);
			Assert.IsNull(stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.Flat, stageDrink.Type);

			stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 3, 2, StageType.IntermediateSprint);
			Assert.AreEqual(2, stageDrink.Order);
		}

		#endregion

		#region Update stage-drink

		[Test]
		public void UpdateStageDrinkEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.UpdateStageDrink(Guid.Empty, 1, null, StageType.Flat));
		}

		[Test]
		public void UpdateStageDrinkNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.UpdateStageDrink(Guid.NewGuid(), 1, null, StageType.Flat));
		}

		[Test]
		public void UpdateStageDrinkZeroNumber()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);

			Assert.Throws<ArgumentOutOfRangeException>(() => StageRepository.UpdateStageDrink(stageDrink.Id, 0, 1, StageType.IntermediateSprint));
		}

		[Test]
		public void UpdateStageDrinkZeroRatio()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);

			Assert.Throws<ArgumentOutOfRangeException>(() => StageRepository.UpdateStageDrink(stageDrink.Id, 5, 0, StageType.IntermediateSprint));
		}

		[Test]
		public void UpdateStageDrinkNotOwner()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => StageRepository.UpdateStageDrink(stageDrink.Id, 5, 1, StageType.IntermediateSprint));

			AsOtherAdmin();
			stageDrink = StageRepository.UpdateStageDrink(stageDrink.Id, 5, 1, StageType.IntermediateSprint);
			Assert.AreEqual(5, stageDrink.NumberToDrink);
			Assert.AreEqual(1, stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.IntermediateSprint, stageDrink.Type);
		}

		[Test]
		public void UpdateStageDrinkOk()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);

			stageDrink = StageRepository.UpdateStageDrink(stageDrink.Id, 5, 1, StageType.IntermediateSprint);
			Assert.AreEqual(stage.Id, stageDrink.StageId);
			Assert.AreEqual(drink.Id, stageDrink.DrinkId);
			Assert.AreEqual(5, stageDrink.NumberToDrink);
			Assert.AreEqual(1, stageDrink.Order);
			Assert.AreEqual(1, stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.IntermediateSprint, stageDrink.Type);
		}

		#endregion

		#region Remove drink from stage

		[Test]
		public void RemoveDrinkFromStageEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.RemoveDrinkFromStage(Guid.Empty));
		}

		[Test]
		public void RemoveDrinkFromStageNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.RemoveDrinkFromStage(Guid.NewGuid()));
		}

		[Test]
		public void RemoveDrinkFromStageNotOwner()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);

			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => StageRepository.RemoveDrinkFromStage(stageDrink.Id));

			AsOtherAdmin();
			StageRepository.RemoveDrinkFromStage(stageDrink.Id);
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageDrinkById(stageDrink.Id));
		}

		[Test]
		public void RemoveDrinkFromStageOk()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);

			StageDrink stageDrink1 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, 3, StageType.Flat);
			Assert.AreEqual(1, stageDrink1.Order);
			StageDrink stageDrink2 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 2, 2, StageType.IntermediateSprint);
			Assert.AreEqual(2, stageDrink2.Order);
			StageDrink stageDrink3 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 3, 1, StageType.Moutain);
			Assert.AreEqual(3, stageDrink3.Order);

			StageRepository.RemoveDrinkFromStage(stageDrink2.Id);
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageDrinkById(stageDrink2.Id));

			stageDrink1 = StageRepository.GetStageDrinkViewById(stageDrink1.Id);
			Assert.AreEqual(1, stageDrink1.Order);
			stageDrink3 = StageRepository.GetStageDrinkViewById(stageDrink3.Id);
			Assert.AreEqual(2, stageDrink3.Order);
		}

		#endregion

		#region Reorder drinks in stage

		[Test]
		public void ChangeDrinkOrderEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.ChangeDrinkOrder(Guid.Empty, 1));
		}

		[Test]
		public void ChangeDrinkOrderNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.ChangeDrinkOrder(Guid.NewGuid(), 1));
		}

		[Test]
		public void ChangeDrinkOrderZeroOrder()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);

			Assert.Throws<ArgumentOutOfRangeException>(() => StageRepository.ChangeDrinkOrder(stageDrink.Id, 0));
		}

		[Test]
		public void ChangeDrinkOrderExceedCount()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);

			Assert.Throws<TourDeFranceException>(() => StageRepository.ChangeDrinkOrder(stageDrink.Id, 2));
		}

		[Test]
		public void ChangeDrinkOrderNotOwner()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink1 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, 2, StageType.Flat);
			StageDrink stageDrink2 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 2, 1, StageType.IntermediateSprint);

			Assert.AreEqual(1, stageDrink1.Order);
			Assert.AreEqual(2, stageDrink2.Order);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => StageRepository.ChangeDrinkOrder(stageDrink1.Id, 2));

			AsOtherAdmin();
			StageRepository.ChangeDrinkOrder(stageDrink1.Id, 2);
			stageDrink1 = StageRepository.GetStageDrinkViewById(stageDrink1.Id);
			Assert.AreEqual(2, stageDrink1.Order);
			stageDrink2 = StageRepository.GetStageDrinkViewById(stageDrink2.Id);
			Assert.AreEqual(1, stageDrink2.Order);
		}

		[Test]
		public void ChangeDrinkOrderOk()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageDrink stageDrink1 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, 4, StageType.Flat);
			StageDrink stageDrink2 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 2, 3, StageType.IntermediateSprint);
			StageDrink stageDrink3 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 3, 2, StageType.Moutain);
			StageDrink stageDrink4 = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 4, 1, StageType.IntermediateSprint);

			Assert.AreEqual(1, stageDrink1.Order);
			Assert.AreEqual(2, stageDrink2.Order);
			Assert.AreEqual(3, stageDrink3.Order);
			Assert.AreEqual(4, stageDrink4.Order);

			StageRepository.ChangeDrinkOrder(stageDrink2.Id, 4);
			stageDrink1 = StageRepository.GetStageDrinkViewById(stageDrink1.Id);
			Assert.AreEqual(1, stageDrink1.Order);
			stageDrink2 = StageRepository.GetStageDrinkViewById(stageDrink2.Id);
			Assert.AreEqual(4, stageDrink2.Order);
			stageDrink3 = StageRepository.GetStageDrinkViewById(stageDrink3.Id);
			Assert.AreEqual(2, stageDrink3.Order);
			stageDrink4 = StageRepository.GetStageDrinkViewById(stageDrink4.Id);
			Assert.AreEqual(3, stageDrink4.Order);

			StageRepository.ChangeDrinkOrder(stageDrink4.Id, 1);
			stageDrink1 = StageRepository.GetStageDrinkViewById(stageDrink1.Id);
			Assert.AreEqual(2, stageDrink1.Order);
			stageDrink2 = StageRepository.GetStageDrinkViewById(stageDrink2.Id);
			Assert.AreEqual(4, stageDrink2.Order);
			stageDrink3 = StageRepository.GetStageDrinkViewById(stageDrink3.Id);
			Assert.AreEqual(3, stageDrink3.Order);
			stageDrink4 = StageRepository.GetStageDrinkViewById(stageDrink4.Id);
			Assert.AreEqual(1, stageDrink4.Order);
		}

		#endregion

		// TODO: other gets
		#region Get stage-drink

		[Test]
		public void GetStageDrinkEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => StageRepository.GetStageDrinkById(Guid.Empty));
		}

		[Test]
		public void GetStageDrinkNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageDrinkById(Guid.NewGuid()));
		}

		[Test]
		public void GetStageDrinkByIdOk()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);

			StageDrink stageDrink = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat);
			Assert.AreEqual(stage.Id, stageDrink.StageId);
			Assert.AreEqual(drink.Id, stageDrink.DrinkId);
			Assert.AreEqual(1, stageDrink.NumberToDrink);
			Assert.AreEqual(1, stageDrink.Order);
			Assert.IsNull(stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.Flat, stageDrink.Type);

			DbStageDrink getStageDrink = StageRepository.GetStageDrinkById(stageDrink.Id);
			Assert.AreEqual(stageDrink.StageId, getStageDrink.StageId);
			Assert.AreEqual(stageDrink.DrinkId, getStageDrink.DrinkId);
			Assert.AreEqual(stageDrink.NumberToDrink, getStageDrink.NumberToDrink);
			Assert.AreEqual(stageDrink.Order, getStageDrink.Order);
			Assert.AreEqual(stageDrink.OverridedVolume, getStageDrink.OverridedVolume);
			Assert.AreEqual(stageDrink.Type, getStageDrink.Type);
		}

		[Test]
		public void CacheOnGetStageDrinkById()
		{
			DbDrink drink = DrinkRepository.CreateDrink("Test", 1, 1, null);
			DbStage stage = StageRepository.CreateStage("Test", 1);
			StageRepository.AddDrinkToStage(stage.Id, drink.Id, 3, 2, StageType.Moutain);

			Guid stageDrinkId = StageRepository.AddDrinkToStage(stage.Id, drink.Id, 1, null, StageType.Flat).Id;
			DbStageDrink stageDrink = StageRepository.GetStageDrinkById(stageDrinkId);
			Assert.AreEqual(stage.Id, stageDrink.StageId);
			Assert.AreEqual(drink.Id, stageDrink.DrinkId);
			Assert.AreEqual(1, stageDrink.NumberToDrink);
			Assert.AreEqual(2, stageDrink.Order);
			Assert.IsNull(stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.Flat, stageDrink.Type);
			CheckCache(stageDrink, stageDrinkId);

			StageRepository.UpdateStageDrink(stageDrink.Id, 5, 1, StageType.IntermediateSprint);
			stageDrink = StageRepository.GetStageDrinkById(stageDrinkId);
			Assert.AreEqual(stage.Id, stageDrink.StageId);
			Assert.AreEqual(drink.Id, stageDrink.DrinkId);
			Assert.AreEqual(5, stageDrink.NumberToDrink);
			Assert.AreEqual(2, stageDrink.Order);
			Assert.AreEqual(1, stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.IntermediateSprint, stageDrink.Type);
			CheckCache(stageDrink, stageDrinkId);

			StageRepository.ChangeDrinkOrder(stageDrink.Id, 1);
			stageDrink = StageRepository.GetStageDrinkById(stageDrinkId);
			Assert.AreEqual(stage.Id, stageDrink.StageId);
			Assert.AreEqual(drink.Id, stageDrink.DrinkId);
			Assert.AreEqual(5, stageDrink.NumberToDrink);
			Assert.AreEqual(1, stageDrink.Order);
			Assert.AreEqual(1, stageDrink.OverridedVolume);
			Assert.AreEqual(StageType.IntermediateSprint, stageDrink.Type);
			CheckCache(stageDrink, stageDrinkId);

			StageRepository.RemoveDrinkFromStage(stageDrinkId);
			Assert.IsNull(GetCacheObject<DbStageDrink>(stageDrinkId));
			Assert.Throws<NotFoundException>(() => StageRepository.GetStageDrinkById(stageDrinkId));
		}

		#endregion
	}
}
