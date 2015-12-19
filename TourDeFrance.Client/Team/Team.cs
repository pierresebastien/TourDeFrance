using System;

namespace TourDeFrance.Client.Team
{
	public class Team
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public Guid OwnerId { get; set; }
	}

	public class CreateUpdateTeam
	{
		public string Name { get; set; }
	}
}
