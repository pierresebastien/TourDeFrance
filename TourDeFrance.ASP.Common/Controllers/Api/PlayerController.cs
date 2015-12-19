using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TourDeFrance.Client.Player;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	public class PlayerController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/players")]
		public IEnumerable<Player> GetAllPlayers()
		{
			return PlayerRepository.GetAllPlayers().Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/players/{playerId}")]
		public Player GetPlayer(Guid playerId)
		{
			return PlayerRepository.GetPlayerById(playerId).ToModel();
		}

		#endregion

		#region POST

		[HttpPost]
		[Route("api/players")]
		public Player CreatePlayer(CreateUpdatePlayer model)
		{
			return
				PlayerRepository.CreatePlayer(model.Nickname, model.FirstName, model.LastName, model.Gender, model.BirthDate,
					model.Height, model.Weight).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/players/{playerId}")]
		public Player UpdatePlayer(Guid playerId, CreateUpdatePlayer model)
		{
			return
				PlayerRepository.UpdatePlayer(playerId, model.Nickname, model.FirstName, model.LastName, model.Gender,
					model.BirthDate, model.Height, model.Weight).ToModel();
		}

		#endregion

		#region DELETE

		[HttpDelete]
		[Route("api/players/{playerId}")]
		public IHttpActionResult DeletePlayer(Guid playerId)
		{
			PlayerRepository.DeletePlayer(playerId);
			return Ok();
		}

		#endregion
	}
}
