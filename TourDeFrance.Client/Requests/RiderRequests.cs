using System;
using TourDeFrance.Client.Enums;
using TourDeFrance.Client.Interfaces;

namespace TourDeFrance.Client.Requests
{
	public class CreateRiderRequest
	{
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Gender Gender { get; set; }

		public DateTime? BirthDate { get; set; }

		public string Nationality { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }

		public byte[] Picture { get; set; }

		public Guid TeamId { get; set; }
	}

	public class UpdateRiderRequest : CreateRiderRequest, IIdentifiable<Guid>
	{
		public Guid Id { get; set; }
	}
}
