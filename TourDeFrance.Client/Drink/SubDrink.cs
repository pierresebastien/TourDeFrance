using System;

namespace TourDeFrance.Client.Drink
{
	public class SubDrink
	{
		public Guid Id { get; set; }

		public Guid DrinkId { get; set;  }

		public string Name { get; set; }

		public decimal? AlcoholByVolume { get; set; }

		public decimal Volume { get; set; }
	}

	public class SubDrinkDefinition
	{
		public Guid DrinkId { get; set; }

		public decimal Volume { get; set; }
	}
}
