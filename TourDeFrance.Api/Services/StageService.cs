using System;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Client.Requests;
using System.Linq;
using TourDeFrance.Client.Responses;

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
			Post["/"] = _ => Negotiate.WithModel(CreateStage(this.BindAndValidate<CreateStageRequest>()));
			Post["/{StageId}/drinks"] = _ => Negotiate.WithModel(AddDrinkToStage(this.BindAndValidate<CreateStageDrinkRequest>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateStage(this.BindAndValidate<UpdateStageRequest>()));
			Put["/drinks/{Id}"] = _ => Negotiate.WithModel(UpdateStageDrink(this.BindAndValidate<UpdateStageDrinkRequest>()));
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

		public Stage CreateStage(CreateStageRequest request)
		{
			return StageRepository.CreateStage(request.Name, request.Duration).ToModel();
		}

		public StageDrink AddDrinkToStage(CreateStageDrinkRequest request)
		{
			return StageRepository.AddDrinkToStage(request.StageId, request.DrinkId, request.NumberToDrink, request.OverridedVolume, request.Type).ToModel();
		}

		public Stage UpdateStage(UpdateStageRequest request)
		{
			return StageRepository.UpdateStage(request.Id, request.Name, request.Duration).ToModel();
		}

		public StageDrink UpdateStageDrink(UpdateStageDrinkRequest request)
		{
			// TODO: only one method?? or 2 methods in api?
			StageRepository.ChangeDrinkOrder(request.Id, request.Order);
			return StageRepository.UpdateStageDrink(request.Id, request.NumberToDrink, request.OverridedVolume, request.Type).ToModel();
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
