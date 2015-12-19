using System;

namespace TourDeFrance.Client.Stage
{
	public class Stage
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid OwnerId { get; set; }

		// NOTE: in seconds
		public int Duration { get; set; }
	}

	public class CreateUpdateStage
	{
		public string Name { get; set; }

		public int Duration { get; set; }
	}
}
