using System;
using TourDeFrance.Client.Enums;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class BaseStageDrinkRequest
	{
		public int NumberToDrink { get; set; }

		public decimal? OverridedVolume { get; set; }

		public StageType Type { get; set; }
	}

	public class CreateStageDrinkRequest : BaseStageDrinkRequest
	{
		public Guid StageId { get; set; }

		public Guid DrinkId { get; set; }
	}

	public class UpdateStageDrinkRequest : BaseStageDrinkRequest, IIdentifiable<Guid>
	{
		public Guid Id { get; set; }

		public int Order { get; set; }
	}
}
