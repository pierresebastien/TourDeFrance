using System;
using TourDeFrance.Client.Enums;

namespace TourDeFrance.Client.Responses
{
	public class StageDrink
	{
		public Guid Id { get; set; }

		public Guid DrinkId { get; set; }

		public string DrinkName { get; set; }

		// NOTE: in percent
		public decimal? AlcoholByVolume { get; set; }

		// NOTE: in centiliter
		public decimal? Volume { get; set; }

		public bool IsComposedDrink { get; set; }

		public Guid StageId { get; set; }

		public decimal? OverridedVolume { get; set; }

		public int NumberToDrink { get; set; }

		public int Order { get; set; }

		public StageType Type { get; set; }

		public decimal GameVolume { get; set; }
	}
}
