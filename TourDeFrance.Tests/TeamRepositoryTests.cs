using System;
using NUnit.Framework;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Exceptions;

namespace TourDeFrance.Tests
{
	public abstract partial class BaseRepositoryTests
	{
		#region Get team

		[Test]
		public void GetTeamEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => TeamRepository.GetTeamById(Guid.Empty));
		}

		[Test]
		public void GetTeamNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => TeamRepository.GetTeamById(Guid.NewGuid()));
		}

		[Test]
		public void GetTeamByIdOk()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			Assert.AreEqual("Test", team.Name);
			Assert.AreEqual(CurrentUser.Id, team.Owner);

			DbTeam getTeam = TeamRepository.GetTeamById(team.Id);
			Assert.AreEqual(team.Name, getTeam.Name);
			Assert.AreEqual(team.Owner, getTeam.Owner);
		}

		[Test]
		public void CacheOnGetTeamById()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			Assert.AreEqual("Test", team.Name);
			Assert.AreEqual(CurrentUser.Id, team.Owner);
			CheckCache(team, team.Id, id => TeamRepository.GetTeamById(id));

			team = TeamRepository.UpdateTeam(team.Id, "Team");
			Assert.AreEqual("Team", team.Name);
			Assert.AreEqual(CurrentUser.Id, team.Owner);
			CheckCache(team, team.Id, id => TeamRepository.GetTeamById(id));

			TeamRepository.DeleteTeam(team.Id);
			Assert.IsNull(GetCacheObject<DbTeam>(team.Id));
			Assert.Throws<NotFoundException>(() => TeamRepository.GetTeamById(team.Id));
		}

		[Test]
		public void GetTeamEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => TeamRepository.GetTeamByName("   "));
		}

		[Test]
		public void GetTeamNonExistentName()
		{
			Assert.Throws<NotFoundException>(() => TeamRepository.GetTeamByName("Test"));
		}

		[Test]
		public void GetTeamByNameOk()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			Assert.AreEqual("Test", team.Name);
			Assert.AreEqual(CurrentUser.Id, team.Owner);

			DbTeam getTeam = TeamRepository.GetTeamByName("Test");
			Assert.AreEqual(team.Id, getTeam.Id);
			Assert.AreEqual(team.Owner, getTeam.Owner);
		}

		#endregion

		#region Create team

		[Test]
		public void CreateTeamEmptyName()
		{
			Assert.Throws<ArgumentNullException>(() => TeamRepository.CreateTeam("   "));
		}

		[Test]
		public void CreateTeamNameAlreadyExists()
		{
			TeamRepository.CreateTeam("Test");
			Assert.Throws<NameAlreadyExistsException>(() => TeamRepository.CreateTeam("Test"));
		}

		[Test]
		public void CreateTeamOk()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			Assert.AreEqual("Test", team.Name);
			Assert.AreEqual(CurrentUser.Id, team.Owner);
		}

		#endregion

		#region Update team

		[Test]
		public void UpdateTeamEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => TeamRepository.UpdateTeam(Guid.Empty, "Test"));
		}

		[Test]
		public void UpdateTeamNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => TeamRepository.UpdateTeam(Guid.NewGuid(), "Test"));
		}

		[Test]
		public void UpdateTeamEmptyName()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			Assert.Throws<ArgumentNullException>(() => TeamRepository.UpdateTeam(team.Id, "   "));
		}

		[Test]
		public void UpdateTeamNameAlreadyExists()
		{
			TeamRepository.CreateTeam("Test");
			DbTeam team = TeamRepository.CreateTeam("Team");
			Assert.Throws<NameAlreadyExistsException>(() => TeamRepository.UpdateTeam(team.Id, "Test"));
		}

		[Test]
		public void UpdateTeamNotOwner()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => TeamRepository.UpdateTeam(team.Id, "Test"));

			AsOtherAdmin();
			team = TeamRepository.UpdateTeam(team.Id, "Team");
			Assert.AreEqual("Team", team.Name);
			Assert.AreNotEqual(CurrentUser.Id, team.Owner);

			AsAdmin();
			Assert.AreEqual(CurrentUser.Id, team.Owner);
		}

		[Test]
		public void UpdateTeamOk()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			Assert.AreEqual("Test", team.Name);
			Assert.AreEqual(CurrentUser.Id, team.Owner);

			team = TeamRepository.UpdateTeam(team.Id, "Team");
			Assert.AreEqual("Team", team.Name);
		}

		#endregion

		#region Delete team

		[Test]
		public void DeleteTeamEmptyId()
		{
			Assert.Throws<ArgumentNullException>(() => TeamRepository.DeleteTeam(Guid.Empty));
		}

		[Test]
		public void DeleteTeamNonExistentId()
		{
			Assert.Throws<NotFoundException>(() => TeamRepository.DeleteTeam(Guid.NewGuid()));
		}

		[Test]
		public void DeleteTeamNotOwner()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");

			AsSimpleUser();
			Assert.Throws<NotOwnerException>(() => TeamRepository.DeleteTeam(team.Id));

			AsOtherAdmin();
			TeamRepository.DeleteTeam(team.Id);
		}

		[Test]
		public void DeleteTeamUsedForRiders()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			RiderRepository.CreateRider("John", "Doe", Gender.Male, DateTime.Today.AddYears(-5), "Belgian", null, null, null, team.Id);
			Assert.Throws<TourDeFranceException>(() => TeamRepository.DeleteTeam(team.Id));
		}

		[Test]
		[Ignore]
		public void DeleteTeamUsedForGameParticipants()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			// TODO: use team for participant
			Assert.Throws<TourDeFranceException>(() => TeamRepository.DeleteTeam(team.Id));
		}

		[Test]
		public void DeleteTeamOk()
		{
			DbTeam team = TeamRepository.CreateTeam("Test");
			TeamRepository.DeleteTeam(team.Id);
			Assert.Throws<NotFoundException>(() => TeamRepository.GetTeamById(team.Id));
		}

		#endregion
	}
}
