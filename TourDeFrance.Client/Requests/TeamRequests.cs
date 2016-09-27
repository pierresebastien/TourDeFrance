using System;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class CreateTeamRequest
	{
		public string Name { get; set; }
	}

	public class UpdateTeamRequest : CreateTeamRequest, IIdentifiable<Guid>
	{
		public Guid Id { get; set; }
	}
}
