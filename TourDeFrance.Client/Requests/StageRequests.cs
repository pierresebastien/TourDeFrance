using System;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class CreateStageRequest
	{
		public string Name { get; set; }

		public int Duration { get; set; }
	}

	public class UpdateStageRequest : CreateStageRequest, IIdentifiable<Guid>
	{
		public Guid Id { get; set; }
	}
}
