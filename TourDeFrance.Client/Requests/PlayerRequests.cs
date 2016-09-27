using System;
using TourDeFrance.Client.Enums;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class CreatePlayerRequest
	{
		public string Nickname { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Gender Gender { get; set; }

		public DateTime? BirthDate { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }
	}

	public class UpdatePlayerRequest : CreatePlayerRequest, IIdentifiable<Guid>
	{
		public Guid Id { get; set; }
	}
}
