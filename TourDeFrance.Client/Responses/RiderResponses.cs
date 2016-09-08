using System;
using TourDeFrance.Client.Enums;

namespace TourDeFrance.Client.Responses
{
	public class Rider
	{
		public Guid Id { get; set; }

		public Guid OwnerId { get; set; }

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
}
