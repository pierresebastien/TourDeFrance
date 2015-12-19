using System;
using System.Collections.Generic;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IRaceRepository
	{
		DbRace GetRaceById(Guid id, bool throwIfNotExist = true);

		DbRace GetRaceByName(string name, bool throwIfNotExist = true);

		IEnumerable<DbRace> GetAllRaces();

		DbRace CreateRace(string name);

		DbRace UpdateRace(Guid id, string name);

		DbRace DeleteRace(Guid id);

		RaceStage AddStageToRace(Guid raceId, Guid stageId);

		RaceStage RemoveStageFromRace(Guid id);

		RaceStage ChangeStageOrder(Guid id, int newOrder);

		DbRaceStage GetRaceStageById(Guid id, bool throwIfNotExist = true);

		IEnumerable<RaceStage> GetStagesForRace(Guid raceId);

		RaceStage GetRaceStageViewById(Guid raceStageId);
	}
}