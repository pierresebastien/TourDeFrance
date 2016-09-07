using TourDeFrance.Client.Drink;

namespace TourDeFrance.Client.Requests
{
	// TODO: validators
	public class CreateDrinkRequest
	{
		public string Name { get; set; }

		public decimal? AlcoholByVolume { get; set; }

		public decimal? Volume { get; set; }

		public SubDrinkDefinition[] SubDrinkDefinitions { get; set; }
	}

	public class UpdateDrinkRequest : ObjectByGuidRequest
	{
		public string Name { get; set; }

		public decimal? AlcoholByVolume { get; set; }

		public decimal? Volume { get; set; }

		public SubDrinkDefinition[] SubDrinkDefinitions { get; set; }
	}
}
