using System;
using NUnit.Framework;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;

namespace TourDeFrance.Tests
{
	public abstract partial class BaseRepositoryTests
	{
		#region Get race

		[Test]
		public void GetRaceEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.GetRaceById(Guid.Empty));
		}

		[Test]
		public void GetRaceNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceById(Guid.NewGuid()));
		}

		[Test]
		public void GetRaceByIdOk()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.AreEqual("Test", race.Name);
			Assert.AreEqual(CurrentUser.Id, race.Owner);

			DbRace getRace = RaceRepository.GetRaceById(race.Id);
			Assert.AreEqual(race.Name, getRace.Name);
			Assert.AreEqual(race.Owner, getRace.Owner);
		}

		[Test]
		public void CacheOnGetRaceById()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.AreEqual("Test", race.Name);
			Assert.AreEqual(CurrentUser.Id, race.Owner);
			CheckCache(race, race.Id, id => RaceRepository.GetRaceById(id));

			race = RaceRepository.UpdateRace(race.Id, "Race");
			Assert.AreEqual("Race", race.Name);
			Assert.AreEqual(CurrentUser.Id, race.Owner);
			CheckCache(race, race.Id, id => RaceRepository.GetRaceById(id));

			RaceRepository.DeleteRace(race.Id);
			Assert.IsNull(GetCacheObject<DbRace>(race.Id));
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceById(race.Id));
		}

		[Test]
		public void GetRaceEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.GetRaceByName("   "));
		}

		[Test]
		public void GetRaceNonExistentName()
		{
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceByName("Test"));
		}

		[Test]
		public void GetRaceByNameOk()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.AreEqual("Test", race.Name);
			Assert.AreEqual(CurrentUser.Id, race.Owner);

			DbRace getRace = RaceRepository.GetRaceByName("Test");
			Assert.AreEqual(race.Id, getRace.Id);
			Assert.AreEqual(race.Owner, getRace.Owner);
		}

		#endregion

		#region Create race

		[Test]
		public void CreateRaceEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.CreateRace("   "));
		}


		[Test]
		public void CreateRaceNameAlreadyExists()
		{
			RaceRepository.CreateRace("Test");
			Assert.Throws<NameAlreadyExistsException>(() => RaceRepository.CreateRace("Test"));
		}

		[Test]
		public void CreateRaceOk()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.AreEqual("Test", race.Name);
			Assert.AreEqual(CurrentUser.Id, race.Owner);
		}

		#endregion

		#region Update race

		[Test]
		public void UpdateRaceEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.UpdateRace(Guid.Empty, "Test"));
		}

		[Test]
		public void UpdateRaceNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => RaceRepository.UpdateRace(Guid.NewGuid(), "Test"));
		}

		[Test]
		public void UpdateRaceEmptyName()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.Throws<ArgumentNullException>(() => RaceRepository.UpdateRace(race.Id, "   "));
		}

		[Test]
		public void UpdateRaceNameAlreadyExists()
		{
			RaceRepository.CreateRace("Test");
			DbRace race = RaceRepository.CreateRace("Race");
			Assert.Throws<NameAlreadyExistsException>(() => RaceRepository.UpdateRace(race.Id, "Test"));
		}

		[Test]
		public void UpdateRaceNotOwner()
		{
			DbRace race = RaceRepository.CreateRace("Test");

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => RaceRepository.UpdateRace(race.Id, "Test"));

			AsOtherAdmin();
			race = RaceRepository.UpdateRace(race.Id, "Race");
			Assert.AreEqual("Race", race.Name);
			Assert.AreNotEqual(CurrentUser.Id, race.Owner);

			AsAdmin();
			Assert.AreEqual(CurrentUser.Id, race.Owner);
		}

		[Test]
		public void UpdateRaceOk()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.AreEqual("Test", race.Name);
			Assert.AreEqual(CurrentUser.Id, race.Owner);

			race = RaceRepository.UpdateRace(race.Id, "Race");
			Assert.AreEqual("Race", race.Name);
		}

		#endregion

		#region Delete race

		[Test]
		public void DeleteRaceEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.DeleteRace(Guid.Empty));
		}

		[Test]
		public void DeleteRaceNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => RaceRepository.DeleteRace(Guid.NewGuid()));
		}

		[Test]
		public void DeleteRaceNotOwner()
		{
			DbRace race = RaceRepository.CreateRace("Test");

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => RaceRepository.DeleteRace(race.Id));

			AsOtherAdmin();
			RaceRepository.DeleteRace(race.Id);
		}

		[Test]
		[Ignore]
		public void DeleteRaceUsedInGames()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			// TODO: create game using this race
			Assert.Throws<TourDeFranceException>(() => RaceRepository.DeleteRace(race.Id));
		}

		[Test]
		public void DeleteRaceOk()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			RaceRepository.DeleteRace(race.Id);
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceById(race.Id));
		}

		#endregion

		#region Add stage to race

		[Test]
		public void AddStageToRaceEmptyRaceId()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<ArgumentNullException>(() => RaceRepository.AddStageToRace(Guid.Empty, stage.Id));
		}

		[Test]
		public void AddStageToRaceEmptyStageId()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.Throws<ArgumentNullException>(() => RaceRepository.AddStageToRace(race.Id, Guid.Empty));
		}

		[Test]
		public void AddStageToRaceNonExistentRaceId()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			Assert.Throws<NotFoundException>(() => RaceRepository.AddStageToRace(Guid.NewGuid(), stage.Id));
		}

		[Test]
		public void AddStageToRaceNonExistentStageId()
		{
			DbRace race = RaceRepository.CreateRace("Test");
			Assert.Throws<NotFoundException>(() => RaceRepository.AddStageToRace(race.Id, Guid.NewGuid()));
		}

		[Test]
		public void AddStageToRaceNotOwner()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			DbRace race = RaceRepository.CreateRace("Test");

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => RaceRepository.AddStageToRace(race.Id, stage.Id));

			AsOtherAdmin();
			RaceStage raceStage = RaceRepository.AddStageToRace(race.Id, stage.Id);
			Assert.AreEqual(race.Id, raceStage.RaceId);
			Assert.AreEqual(stage.Id, raceStage.StageId);
			Assert.AreEqual(1, raceStage.Order);
		}

		[Test]
		public void AddStageToRaceAlreadyAdded()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			DbRace race = RaceRepository.CreateRace("Test");
			RaceRepository.AddStageToRace(race.Id, stage.Id);
			Assert.Throws<TourDeFranceException>(() => RaceRepository.AddStageToRace(race.Id, stage.Id));
		}

		[Test]
		public void AddStageToRaceOk()
		{
			DbStage stage1 = StageRepository.CreateStage("Test1", 1);
			DbStage stage2 = StageRepository.CreateStage("Test2", 2);
			DbRace race = RaceRepository.CreateRace("Test");

			RaceStage raceStage = RaceRepository.AddStageToRace(race.Id, stage1.Id);
			Assert.AreEqual(race.Id, raceStage.RaceId);
			Assert.AreEqual(stage1.Id, raceStage.StageId);
			Assert.AreEqual(1, raceStage.Order);

			raceStage = RaceRepository.AddStageToRace(race.Id, stage2.Id);
			Assert.AreEqual(2, raceStage.Order);
		}

		#endregion

		#region Remove stage from race

		[Test]
		public void RemoveStageFromRaceEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.RemoveStageFromRace(Guid.Empty));
		}

		[Test]
		public void RemoveStageFromRaceNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => RaceRepository.RemoveStageFromRace(Guid.NewGuid()));
		}

		[Test]
		public void RemoveStageFromRaceNotOwner()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			DbRace race = RaceRepository.CreateRace("Test");

			RaceStage stageRace = RaceRepository.AddStageToRace(race.Id, stage.Id);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => RaceRepository.RemoveStageFromRace(stageRace.Id));

			AsOtherAdmin();
			RaceRepository.RemoveStageFromRace(stageRace.Id);
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceStageById(stageRace.Id));
		}

		[Test]
		public void RemoveStageFromRaceOk()
		{
			DbStage stage1 = StageRepository.CreateStage("Test1", 1);
			DbStage stage2 = StageRepository.CreateStage("Test2", 2);
			DbStage stage3 = StageRepository.CreateStage("Test3", 3);
			DbRace race = RaceRepository.CreateRace("Test");

			RaceStage stageRace1 = RaceRepository.AddStageToRace(race.Id, stage1.Id);
			Assert.AreEqual(1, stageRace1.Order);
			RaceStage stageRace2 = RaceRepository.AddStageToRace(race.Id, stage2.Id);
			Assert.AreEqual(2, stageRace2.Order);
			RaceStage stageRace3 = RaceRepository.AddStageToRace(race.Id, stage3.Id);
			Assert.AreEqual(3, stageRace3.Order);

			RaceRepository.RemoveStageFromRace(stageRace2.Id);
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceStageById(stageRace2.Id));

			stageRace1 = RaceRepository.GetRaceStageViewById(stageRace1.Id);
			Assert.AreEqual(1, stageRace1.Order);
			stageRace3 = RaceRepository.GetRaceStageViewById(stageRace3.Id);
			Assert.AreEqual(2, stageRace3.Order);

			stageRace2 = RaceRepository.AddStageToRace(race.Id, stage2.Id);
			Assert.AreEqual(3, stageRace2.Order);
		}

		#endregion

		#region Reorder stages in race

		[Test]
		public void ChangeStageOrderEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.ChangeStageOrder(Guid.Empty, 1));
		}

		[Test]
		public void ChangeStageOrderNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => RaceRepository.ChangeStageOrder(Guid.NewGuid(), 1));
		}

		[Test]
		public void ChangeStageOrderZeroOrder()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			DbRace race = RaceRepository.CreateRace("Test");
			RaceStage stageRace = RaceRepository.AddStageToRace(race.Id, stage.Id);

			Assert.Throws<ArgumentOutOfRangeException>(() => RaceRepository.ChangeStageOrder(stageRace.Id, 0));
		}

		[Test]
		public void ChangeStageOrderExceedCount()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			DbRace race = RaceRepository.CreateRace("Test");
			RaceStage stageRace = RaceRepository.AddStageToRace(race.Id, stage.Id);

			Assert.Throws<TourDeFranceException>(() => RaceRepository.ChangeStageOrder(stageRace.Id, 2));
		}

		[Test]
		public void ChangeStageOrderNotOwner()
		{
			DbStage stage1 = StageRepository.CreateStage("Test1", 1);
			DbStage stage2 = StageRepository.CreateStage("Test2", 2);
			DbRace race = RaceRepository.CreateRace("Test");
			RaceStage stageRace1 = RaceRepository.AddStageToRace(race.Id, stage1.Id);
			RaceStage stageRace2 = RaceRepository.AddStageToRace(race.Id, stage2.Id);

			Assert.AreEqual(1, stageRace1.Order);
			Assert.AreEqual(2, stageRace2.Order);

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => RaceRepository.ChangeStageOrder(stageRace1.Id, 2));

			AsOtherAdmin();
			RaceRepository.ChangeStageOrder(stageRace1.Id, 2);
			stageRace1 = RaceRepository.GetRaceStageViewById(stageRace1.Id);
			Assert.AreEqual(2, stageRace1.Order);
			stageRace2 = RaceRepository.GetRaceStageViewById(stageRace2.Id);
			Assert.AreEqual(1, stageRace2.Order);
		}

		[Test]
		public void ChangeStageOrderOk()
		{
			DbStage stage1 = StageRepository.CreateStage("Test1", 1);
			DbStage stage2 = StageRepository.CreateStage("Test2", 2);
			DbStage stage3 = StageRepository.CreateStage("Test3", 3);
			DbStage stage4 = StageRepository.CreateStage("Test4", 4);
			DbRace race = RaceRepository.CreateRace("Test");
			RaceStage stageRace1 = RaceRepository.AddStageToRace(race.Id, stage1.Id);
			RaceStage stageRace2 = RaceRepository.AddStageToRace(race.Id, stage2.Id);
			RaceStage stageRace3 = RaceRepository.AddStageToRace(race.Id, stage3.Id);
			RaceStage stageRace4 = RaceRepository.AddStageToRace(race.Id, stage4.Id);

			Assert.AreEqual(1, stageRace1.Order);
			Assert.AreEqual(2, stageRace2.Order);
			Assert.AreEqual(3, stageRace3.Order);
			Assert.AreEqual(4, stageRace4.Order);

			RaceRepository.ChangeStageOrder(stageRace2.Id, 4);
			stageRace1 = RaceRepository.GetRaceStageViewById(stageRace1.Id);
			Assert.AreEqual(1, stageRace1.Order);
			stageRace2 = RaceRepository.GetRaceStageViewById(stageRace2.Id);
			Assert.AreEqual(4, stageRace2.Order);
			stageRace3 = RaceRepository.GetRaceStageViewById(stageRace3.Id);
			Assert.AreEqual(2, stageRace3.Order);
			stageRace4 = RaceRepository.GetRaceStageViewById(stageRace4.Id);
			Assert.AreEqual(3, stageRace4.Order);

			RaceRepository.ChangeStageOrder(stageRace4.Id, 1);
			stageRace1 = RaceRepository.GetRaceStageViewById(stageRace1.Id);
			Assert.AreEqual(2, stageRace1.Order);
			stageRace2 = RaceRepository.GetRaceStageViewById(stageRace2.Id);
			Assert.AreEqual(4, stageRace2.Order);
			stageRace3 = RaceRepository.GetRaceStageViewById(stageRace3.Id);
			Assert.AreEqual(3, stageRace3.Order);
			stageRace4 = RaceRepository.GetRaceStageViewById(stageRace4.Id);
			Assert.AreEqual(1, stageRace4.Order);
		}

		#endregion

		#region Get race-stage

		[Test]
		public void GetRaceStageEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => RaceRepository.GetRaceStageById(Guid.Empty));
		}

		[Test]
		public void GetRaceStageNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceStageById(Guid.NewGuid()));
		}

		[Test]
		public void GetRaceStageByIdOk()
		{
			DbStage stage = StageRepository.CreateStage("Test", 1);
			DbRace race = RaceRepository.CreateRace("Test");

			RaceStage raceStage = RaceRepository.AddStageToRace(race.Id, stage.Id);
			Assert.AreEqual(race.Id, raceStage.RaceId);
			Assert.AreEqual(stage.Id, raceStage.StageId);
			Assert.AreEqual(1, raceStage.Order);

			DbRaceStage getRaceStage = RaceRepository.GetRaceStageById(raceStage.Id);
			Assert.AreEqual(raceStage.RaceId, getRaceStage.RaceId);
			Assert.AreEqual(raceStage.StageId, getRaceStage.StageId);
			Assert.AreEqual(raceStage.Order, getRaceStage.Order);
		}

		[Test]
		public void CacheOnGetRaceStageById()
		{
			DbStage stage1 = StageRepository.CreateStage("Test1", 1);
			DbStage stage2 = StageRepository.CreateStage("Test2", 2);
			DbRace race = RaceRepository.CreateRace("Test");
			RaceRepository.AddStageToRace(race.Id, stage1.Id);

			Guid raceStageId = RaceRepository.AddStageToRace(race.Id, stage2.Id).Id;
			DbRaceStage raceStage = RaceRepository.GetRaceStageById(raceStageId);
			Assert.AreEqual(race.Id, raceStage.RaceId);
			Assert.AreEqual(stage2.Id, raceStage.StageId);
			Assert.AreEqual(2, raceStage.Order);
			CheckCache(raceStage, raceStageId);

			RaceRepository.ChangeStageOrder(raceStage.Id, 1);
			raceStage = RaceRepository.GetRaceStageById(raceStageId);
			Assert.AreEqual(race.Id, raceStage.RaceId);
			Assert.AreEqual(stage2.Id, raceStage.StageId);
			Assert.AreEqual(1, raceStage.Order);
			CheckCache(raceStage, raceStageId);

			RaceRepository.RemoveStageFromRace(raceStageId);
			Assert.IsNull(GetCacheObject<DbStageDrink>(raceStageId));
			Assert.Throws<NotFoundException>(() => RaceRepository.GetRaceStageById(raceStageId));
		}

		#endregion
	}
}
