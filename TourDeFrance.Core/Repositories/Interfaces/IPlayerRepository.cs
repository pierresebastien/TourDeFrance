using System;
using System.Collections.Generic;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IPlayerRepository
	{
		DbPlayer GetPlayerById(Guid id, bool throwIfNotExist = true);

		DbPlayer GetPlayerByName(string firstName, string lastName, bool throwIfNotExist = true);

		IEnumerable<DbPlayer> GetAllPlayers();

		DbPlayer CreatePlayer(string nickname, string firstName, string lastName, Gender gender, DateTime? birthDate,
			decimal? height, decimal? weight);

		DbPlayer UpdatePlayer(Guid id, string nickname, string firstName, string lastName, Gender gender, DateTime? birthDate,
			decimal? height, decimal? weight);

		DbPlayer DeletePlayer(Guid id);
	}
}