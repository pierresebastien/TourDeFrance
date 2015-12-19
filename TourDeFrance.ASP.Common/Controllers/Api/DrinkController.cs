using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TourDeFrance.Client.Drink;

namespace TourDeFrance.ASP.Common.Controllers.Api
{
	public class DrinkController : BaseApiController
	{
		#region GET

		[HttpGet]
		[Route("api/drinks")]
		public IEnumerable<Drink> GetAllDrinks()
		{
			return DrinkRepository.GetAllDrinks().Select(x => x.ToModel());
		}

		[HttpGet]
		[Route("api/drinks/{drinkId}")]
		public Drink GetDrink(Guid drinkId)
		{
			return DrinkRepository.GetDrinkById(drinkId).ToModel();
		}

		#endregion

		#region POST

		[HttpPost]
		[Route("api/drinks")]
		public Drink CreateDrink(CreateUpdateDrink model)
		{
			return
				DrinkRepository.CreateDrink(model.Name, model.AlcoholByVolume, model.Volume, model.SubDrinkDefinitions).ToModel();
		}

		#endregion

		#region PUT

		[HttpPut]
		[Route("api/drinks/{drinkId}")]
		public Drink UpdateDrink(Guid drinkId, CreateUpdateDrink model)
		{
			return
				DrinkRepository.UpdateDrink(drinkId, model.Name, model.AlcoholByVolume, model.Volume, model.SubDrinkDefinitions)
					.ToModel();
		}

		#endregion

		#region DELETE

		[HttpDelete]
		[Route("api/drinks/{drinkId}")]
		public IHttpActionResult DeleteDrink(Guid drinkId)
		{
			DrinkRepository.DeleteDrink(drinkId);
			return Ok();
		}

		#endregion
	}
}
