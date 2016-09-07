using System;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using ServiceStack.ServiceHost;
using TourDeFrance.Client.Race;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Services
{
	public class RaceService : BaseService
	{
		public RaceService() : base("/races")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllRaces());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetRace(this.BindAndValidate<ObjectByGuidRequest>()));
			Get["/{Id}/stages"] = _ => Negotiate.WithModel(GetRaceStages(this.BindAndValidate<ObjectByGuidRequest>()));
			Get["/stages/{Id}"] = _ => Negotiate.WithModel(GetRaceStage(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateRace(this.BindAndValidate<>()));
			Post["/{Id}/stages"] = _ => Negotiate.WithModel(AddStageToRace(this.BindAndValidate<>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateRace(this.BindAndValidate<>()));
			Put["/stages/{Id}"] = _ => Negotiate.WithModel(UpdateRaceStage(this.BindAndValidate<>()));
			Delete["/{Id}"] = _ => Negotiate.WithModel(DeleteRace(this.BindAndValidate<ObjectByGuidRequest>()));
			Delete["/stages/{Id}"] = _ => Negotiate.WithModel(DeleteRaceStage(this.BindAndValidate<ObjectByGuidRequest>()));
		}

		public Race[] GetAllRaces()
		{
			return RaceRepository.GetAllRaces().Select(x => x.ToModel()).ToArray();
		}

		public Race GetRace(ObjectByGuidRequest request)
		{
			return RaceRepository.GetRaceById(request.Id).ToModel();
		}

		public RaceStage[] GetRaceStages(ObjectByGuidRequest request)
		{
			return RaceRepository.GetStagesForRace(request.Id).Select(x => x.ToModel()).ToArray();
		}

		public RaceStage GetRaceStage(ObjectByGuidRequest request)
		{
			return RaceRepository.GetRaceStageViewById(request.Id).ToModel();
		}

		public Race CreateRace(CreateUpdateRace model)
		{
			return RaceRepository.CreateRace(model.Name).ToModel();
		}

		public RaceStage AddStageToRace(Guid raceId, CreateRaceStage model)
		{
			return RaceRepository.AddStageToRace(raceId, model.StageId).ToModel();
		}

		public Race UpdateRace(Guid raceId, CreateUpdateRace model)
		{
			return RaceRepository.UpdateRace(raceId, model.Name).ToModel();
		}

		public RaceStage UpdateRaceStage(Guid raceStageId, int order)
		{
			return RaceRepository.ChangeStageOrder(raceStageId, order).ToModel();
		}

		public Race DeleteRace(ObjectByGuidRequest request)
		{
			return RaceRepository.DeleteRace(request.Id).ToModel();
		}

		public RaceStage DeleteRaceStage(ObjectByGuidRequest request)
		{
			return RaceRepository.RemoveStageFromRace(request.Id).ToModel();
		}
	}
}
