using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TourDeFrance.Client.Team;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	public class TeamController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/teams")]
		public IEnumerable<Team> GetAllTeams()
		{
			return TeamRepository.GetAllTeams().Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/teams/{teamId}")]
		public Team GetTeam(Guid teamId)
		{
			return TeamRepository.GetTeamById(teamId).ToModel();
		}

		#endregion

		#region POST

		[HttpPost]
		[Route("api/teams")]
		public Team CreateTeam(CreateUpdateTeam model)
		{
			return TeamRepository.CreateTeam(model.Name).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/teams/{teamId}")]
		public Team UpdateTeam(Guid teamId, CreateUpdateTeam model)
		{
			return TeamRepository.UpdateTeam(teamId, model.Name).ToModel();
		}

		#endregion

		#region DELETE

		[HttpDelete]
		[Route("api/teams/{teamId}")]
		public Team DeleteTeam(Guid teamId)
		{
			return TeamRepository.DeleteTeam(teamId).ToModel();
		}

		#endregion
	}
}
