using System;
using System.Collections.Generic;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface ITeamRepository
	{
		DbTeam GetTeamById(Guid id, bool throwIfNotExist = true);

		DbTeam GetTeamByName(string name, bool throwIfNotExist = true);

		IEnumerable<DbTeam> GetAllTeams();

		DbTeam CreateTeam(string name);

		DbTeam UpdateTeam(Guid id, string name);

		DbTeam DeleteTeam(Guid id);
	}
}