using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Client.Requests;
using TourDeFrance.Client.Team;

namespace TourDeFrance.Api.Services
{
	public class TeamService : BaseService
	{
		public TeamService() : base("/teams")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllTeams());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetTeam(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateTeam(this.BindAndValidate<>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateTeam(this.BindAndValidate<>()));
			Delete["/{Id}"] = _ => Negotiate.WithModel(DeleteTeam(this.BindAndValidate<ObjectByGuidRequest>()));
		}

		public Team[] GetAllTeams()
		{
			return TeamRepository.GetAllTeams().Select(x => x.ToModel()).ToArray();
		}

		public Team GetTeam(ObjectByGuidRequest request)
		{
			return TeamRepository.GetTeamById(request.Id).ToModel();
		}

		public Team CreateTeam(CreateUpdateTeam request)
		{
			return TeamRepository.CreateTeam(request.Name).ToModel();
		}

		public Team UpdateTeam(Guid teamId, CreateUpdateTeam request)
		{
			return TeamRepository.UpdateTeam(teamId, request.Name).ToModel();
		}

		public Team DeleteTeam(ObjectByGuidRequest request)
		{
			return TeamRepository.DeleteTeam(request.Id).ToModel();
		}
	}
}
