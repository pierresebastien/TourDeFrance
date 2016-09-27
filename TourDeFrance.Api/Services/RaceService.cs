using System.Linq;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Client.Requests;
using TourDeFrance.Client.Responses;

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
			Post["/"] = _ => Negotiate.WithModel(CreateRace(this.BindAndValidate<CreateRaceRequest>()));
			Post["/{RaceId}/stages"] = _ => Negotiate.WithModel(AddStageToRace(this.BindAndValidate<CreateRaceStageRequest>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateRace(this.BindAndValidate<UpdateRaceRequest>()));
			Put["/stages/{Id}"] = _ => Negotiate.WithModel(UpdateRaceStage(this.BindAndValidate<UpdateRaceStageRequest>()));
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

		public Race CreateRace(CreateRaceRequest request)
		{
			return RaceRepository.CreateRace(request.Name).ToModel();
		}

		public RaceStage AddStageToRace(CreateRaceStageRequest request)
		{
			return RaceRepository.AddStageToRace(request.RaceId, request.StageId).ToModel();
		}

		public Race UpdateRace(UpdateRaceRequest request)
		{
			return RaceRepository.UpdateRace(request.Id, request.Name).ToModel();
		}

		public RaceStage UpdateRaceStage(UpdateRaceStageRequest request)
		{
			return RaceRepository.ChangeStageOrder(request.Id, request.Order).ToModel();
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
