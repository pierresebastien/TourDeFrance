﻿using System;

namespace TourDeFrance.Client.Responses
{
	public class Drink
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid OwnerId { get; set; }

		// NOTE: in percent
		public decimal? AlcoholByVolume { get; set; }

		// NOTE: in centiliter
		public decimal? Volume { get; set; }

		public SubDrink[] SubDrinks { get; set; }

		public bool IsComposedDrink { get; set; }

		public decimal CalculatedVolume { get; set; }

		public decimal CalculatedAlcoholByVolume { get; set; }
	}

	public class SubDrink
	{
		public Guid Id { get; set; }

		public Guid DrinkId { get; set; }

		public string Name { get; set; }

		public decimal? AlcoholByVolume { get; set; }

		public decimal Volume { get; set; }
	}
}
