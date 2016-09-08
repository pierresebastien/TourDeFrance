using System;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class CreateRaceStageRequest
	{
		public Guid RaceId { get; set; }

		public Guid StageId { get; set; }
	}

	public class UpdateRaceStageRequest : IIdentifiable<Guid>
	{
		public Guid Id { get; set; }

		public int Order { get; set; }
	}
}
