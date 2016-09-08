using System;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class CreateRaceRequest
	{
		public string Name { get; set; }
	}

	public class UpdateRaceRequest : CreateRaceRequest, IIdentifiable<Guid>
	{
		public Guid Id { get; set; }
	}
}
