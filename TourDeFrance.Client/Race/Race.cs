using System;

namespace TourDeFrance.Client.Race
{
	public class Race
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid OwnerId { get; set; }
	}

	public class CreateUpdateRace
	{
		public string Name { get; set; }
	}
}
