using System;
using System.Collections.Generic;
using TourDeFrance.Client.Requests;
using Drink = TourDeFrance.Core.Business.Drink;

namespace TourDeFrance.Core.Repositories.Interfaces
{
	public interface IDrinkRepository
	{
		Drink GetDrinkById(Guid id, bool throwIfNotExist = true);

		Drink GetDrinkByName(string name, bool throwIfNotExist = true);

		IEnumerable<Drink> GetAllDrinks();

		Drink CreateDrink(string name, decimal? alcoholByVolume, decimal? volume, IEnumerable<SubDrinkDefinition> subDrinks);

		Drink UpdateDrink(Guid id, string name, decimal? alcoholByVolume, decimal? volume, IEnumerable<SubDrinkDefinition> subDrinks);

		Drink DeleteDrink(Guid id);
	}
}