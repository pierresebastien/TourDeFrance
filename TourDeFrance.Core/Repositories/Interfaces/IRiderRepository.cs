using System;
using System.Collections.Generic;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IRiderRepository
	{
		DbRider GetRiderById(Guid id, bool throwIfNotExist = true);

		IEnumerable<DbRider> GetAllRiders();

		IEnumerable<DbRider> GetRidersForTeam(Guid teamId);

		DbRider CreateRider(string firstName, string lastName, Gender gender, DateTime? birthDate, string nationality,
			decimal? height, decimal? weight, byte[] picture, Guid teamId);

		DbRider UpdateRider(Guid id, string firstName, string lastName, Gender gender, DateTime? birthDate, string nationality,
			decimal? height, decimal? weight, byte[] picture, Guid teamId);

		DbRider DeleteRider(Guid id);
	}
}
