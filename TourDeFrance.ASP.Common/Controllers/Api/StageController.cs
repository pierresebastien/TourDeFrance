using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TourDeFrance.Client.Stage;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	public class StageController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/stages")]
		public IEnumerable<Stage> GetAllStages()
		{
			return StageRepository.GetAllStages().Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/stages/{stageId}")]
		public Stage GetStage(Guid stageId)
		{
			return StageRepository.GetStageById(stageId).ToModel();
		}

		[HttpGet]
		[Route("api/stages/{stageId}/drinks")]
		public IEnumerable<StageDrink> GetStagesDrinks(Guid stageId)
		{
			return StageRepository.GetDrinksForStage(stageId).Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/stagedrinks/{stageDrinkId}")]
		public StageDrink GetStagesDrink(Guid stageDrinkId)
		{
			return StageRepository.GetStageDrinkViewById(stageDrinkId).ToModel();
		}

		#endregion

		#region POST

		[HttpPost]
		[Route("api/stages")]
		public Stage CreateStage(CreateUpdateStage model)
		{
			return StageRepository.CreateStage(model.Name, model.Duration).ToModel();
		}

		[HttpPost]
		[Route("api/stages/{stageId}/drinks")]
		public StageDrink AddDrinkToStage(Guid stageId, CreateStageDrink model)
		{
			return StageRepository.AddDrinkToStage(stageId, model.DrinkId, model.NumberToDrink, model.OverridedVolume, model.Type).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/stages/{stageId}")]
		public Stage UpdateStage(Guid stageId, CreateUpdateStage model)
		{
			return StageRepository.UpdateStage(stageId, model.Name, model.Duration).ToModel();
		}

		[HttpPut]
		[Route("api/stagedrinks/{stageDrinkId}")]
		public StageDrink UpdateStageDrink(Guid stageDrinkId, UpdateStageDrink model)
		{
			return StageRepository.UpdateStageDrink(stageDrinkId, model.NumberToDrink, model.OverridedVolume, model.Type).ToModel();
		}

		[HttpPut]
		[Route("api/stagedrinks/{stageDrinkId}/order/{order}")]
		public StageDrink UpdateStageDrink(Guid stageDrinkId, int order)
		{
			return StageRepository.ChangeDrinkOrder(stageDrinkId, order).ToModel();
		}

		#endregion

		#region DELETE

		[HttpDelete]
		[Route("api/stages/{stageId}")]
		public IHttpActionResult DeleteDrink(Guid stageId)
		{
			StageRepository.DeleteStage(stageId);
			return Ok();
		}

		[HttpDelete]
		[Route("api/stagedrinks/{stageDrinkId}")]
		public IHttpActionResult DeleteStageDrink(Guid stageDrinkId)
		{
			StageRepository.RemoveDrinkFromStage(stageDrinkId);
			return Ok();
		}

		#endregion
	}
}
