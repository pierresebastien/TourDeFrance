using System;

namespace TourDeFrance.Client.Race
{
	public class RaceStage
	{
		public Guid Id { get; set; }

		public Guid StageId { get; set; }

		public Guid RaceId { get; set; }

		public int Order { get; set; }

		public string RaceName { get; set; }

		public int Duration { get; set; }
	}

	public class CreateRaceStage
	{
		public Guid StageId { get; set; }
	}
}
