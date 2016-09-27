using System.Linq;
using TourDeFrance.Client.Requests;
using Nancy;
using Nancy.ModelBinding;
using TourDeFrance.Api.Exceptions;
using TourDeFrance.Client.Responses;

namespace TourDeFrance.Api.Services
{
	public class DrinkService : BaseService
	{
		public DrinkService() : base("/drinks")
		{
			Get["/"] = _ => Negotiate.WithModel(GetAllDrinks());
			Get["/{Id}"] = _ => Negotiate.WithModel(GetDrink(this.BindAndValidate<ObjectByGuidRequest>()));
			Post["/"] = _ => Negotiate.WithModel(CreateDrink(this.BindAndValidate<CreateDrinkRequest>()));
			Put["/{Id}"] = _ => Negotiate.WithModel(UpdateDrink(this.BindAndValidate<UpdateDrinkRequest>()));
			Delete["/{Id}"] = _ => Negotiate.WithModel(DeleteDrink(this.BindAndValidate<ObjectByGuidRequest>()));
		}

		public Drink[] GetAllDrinks()
		{
			// TODO: to add in all methods => create own extension method or check if before in base service can be useful
			if (!ModelValidationResult.IsValid)
			{
				throw new BadRequestException(ModelValidationResult);
			}
			return DrinkRepository.GetAllDrinks().Select(x => x.ToModel()).ToArray();
		}

		public Drink GetDrink(ObjectByGuidRequest request)
		{
			return DrinkRepository.GetDrinkById(request.Id).ToModel();
		}

		public Drink CreateDrink(CreateDrinkRequest request)
		{
			return
				DrinkRepository.CreateDrink(request.Name, request.AlcoholByVolume, request.Volume, request.SubDrinkDefinitions)
					.ToModel();
		}

		public Drink UpdateDrink(UpdateDrinkRequest request)
		{
			return
				DrinkRepository.UpdateDrink(request.Id, request.Name, request.AlcoholByVolume, request.Volume, request.SubDrinkDefinitions)
					.ToModel();
		}

		public Drink DeleteDrink(ObjectByGuidRequest request)
		{
			return DrinkRepository.DeleteDrink(request.Id).ToModel();
		}
	}
}
