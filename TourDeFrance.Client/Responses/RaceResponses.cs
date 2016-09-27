using System;

namespace TourDeFrance.Client.Responses
{
	public class Race
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid OwnerId { get; set; }
	}
}
