using System;
using System.Collections.Generic;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IStageRepository
	{
		DbStage GetStageById(Guid id, bool throwIfNotExist = true);

		DbStage GetStageByName(string name, bool throwIfNotExist = true);

		IEnumerable<DbStage> GetAllStages();

		DbStage CreateStage(string name, int timeInSeconds);

		DbStage UpdateStage(Guid id, string name, int timeInSeconds);

		DbStage DeleteStage(Guid id);

		StageDrink AddDrinkToStage(Guid stageId, Guid drinkId, int numberToDrink, decimal? overridedVolume, StageType type);

		StageDrink UpdateStageDrink(Guid id, int numberToDrink, decimal? overridedVolume, StageType type);

		StageDrink RemoveDrinkFromStage(Guid id);

		StageDrink ChangeDrinkOrder(Guid id, int newOrder);

		DbStageDrink GetStageDrinkById(Guid id, bool throwIfNotExist = true);

		StageDrink GetStageDrinkViewById(Guid stageDrinkId);

		IEnumerable<StageDrink> GetDrinksForStage(Guid stageId);
	}
}