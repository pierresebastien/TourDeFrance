using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TourDeFrance.Client.Race;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	public class RaceController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/races")]
		public IEnumerable<Race> GetAllRaces()
		{
			return RaceRepository.GetAllRaces().Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/races/{raceId}")]
		public Race GetRace(Guid raceId)
		{
			return RaceRepository.GetRaceById(raceId).ToModel();
		}

		[HttpGet]
		[Route("api/races/{raceId}/stages")]
		public IEnumerable<RaceStage> GetRaceStages(Guid raceId)
		{
			return RaceRepository.GetStagesForRace(raceId).Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/racestages/{raceStageId}")]
		public RaceStage GetStagesDrink(Guid raceStageId)
		{
			return RaceRepository.GetRaceStageViewById(raceStageId).ToModel();
		}

		#endregion

		#region POST

		[HttpPost]
		[Route("api/races")]
		public Race CreateRace(CreateUpdateRace model)
		{
			return RaceRepository.CreateRace(model.Name).ToModel();
		}

		[HttpPost]
		[Route("api/races/{raceId}/stages")]
		public RaceStage AddStageToRace(Guid raceId, CreateRaceStage model)
		{
			return RaceRepository.AddStageToRace(raceId, model.StageId).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/races/{raceId}")]
		public Race UpdateRace(Guid raceId, CreateUpdateRace model)
		{
			return RaceRepository.UpdateRace(raceId, model.Name).ToModel();
		}

		[HttpPut]
		[Route("api/racestages/{raceStageId}/order/{order}")]
		public RaceStage UpdateStageDrink(Guid raceStageId, int order)
		{
			return RaceRepository.ChangeStageOrder(raceStageId, order).ToModel();
		}

		#endregion

		#region DELETE

		[HttpDelete]
		[Route("api/races/{raceId}")]
		public IHttpActionResult DeleteRace(Guid raceId)
		{
			RaceRepository.DeleteRace(raceId);
			return Ok();
		}

		[HttpDelete]
		[Route("api/racestages/{raceStageId}")]
		public IHttpActionResult DeleteRaceStage(Guid raceStageId)
		{
			RaceRepository.RemoveStageFromRace(raceStageId);
			return Ok();
		}

		#endregion
	}
}
