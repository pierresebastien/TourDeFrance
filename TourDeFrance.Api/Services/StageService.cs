using System;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Client.Requests;
using TourDeFrance.Client.Stage;
using System.Linq;

namespace TourDeFrance.Api.Services
{
	public class StageService : BaseService
	{
		public StageService() : base("/stages")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllStages());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetStage(this.BindAndValidate<ObjectByGuidRequest>()));
			Get["/{Id}/drinks"] = _ => Negotiate.WithModel(GetStageDrinks(this.BindAndValidate<ObjectByGuidRequest>()));
			Get["/drinks/{Id}"] = _ => Negotiate.WithModel(GetStageDrink(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateStage(this.BindAndValidate<>()));
			Post["/{Id}/drinks"] = _ => Negotiate.WithModel(AddDrinkToStage(this.BindAndValidate<>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateStage(this.BindAndValidate<>()));
			Put["/drinks/{Id}"] = _ => Negotiate.WithModel(UpdateStageDrink(this.BindAndValidate<>()));
			Delete["/{Id}"] = _ => Negotiate.WithModel(DeleteStage(this.BindAndValidate<ObjectByGuidRequest>()));
			Delete["/drinks/{Id}"] = _ => Negotiate.WithModel(DeleteStageDrink(this.BindAndValidate<ObjectByGuidRequest>()));
		}

		public Stage[] GetAllStages()
		{
			return StageRepository.GetAllStages().Select(x => x.ToModel()).ToArray();
		}

		public Stage GetStage(ObjectByGuidRequest request)
		{
			return StageRepository.GetStageById(request.Id).ToModel();
		}

		public StageDrink[] GetStageDrinks(ObjectByGuidRequest request)
		{
			return StageRepository.GetDrinksForStage(request.Id).Select(x => x.ToModel()).ToArray();
		}

		public StageDrink GetStageDrink(ObjectByGuidRequest request)
		{
			return StageRepository.GetStageDrinkViewById(request.Id).ToModel();
		}

		public Stage CreateStage(CreateUpdateStage request)
		{
			return StageRepository.CreateStage(request.Name, request.Duration).ToModel();
		}

		public StageDrink AddDrinkToStage(Guid stageId, CreateStageDrink request)
		{
			return StageRepository.AddDrinkToStage(stageId, request.DrinkId, request.NumberToDrink, request.OverridedVolume, request.Type).ToModel();
		}

		public Stage UpdateStage(Guid stageId, CreateUpdateStage model)
		{
			return StageRepository.UpdateStage(stageId, model.Name, model.Duration).ToModel();
		}

		public StageDrink UpdateStageDrink(Guid stageDrinkId, UpdateStageDrink request)
		{
			// TODO: StageRepository.ChangeDrinkOrder(stageDrinkId, order).ToModel();
			return StageRepository.UpdateStageDrink(stageDrinkId, request.NumberToDrink, request.OverridedVolume, request.Type).ToModel();
		}

		public Stage DeleteStage(ObjectByGuidRequest request)
		{
			return StageRepository.DeleteStage(request.Id).ToModel();
		}

		public StageDrink DeleteStageDrink(ObjectByGuidRequest request)
		{
			return StageRepository.RemoveDrinkFromStage(request.Id).ToModel();
		}
	}
}
