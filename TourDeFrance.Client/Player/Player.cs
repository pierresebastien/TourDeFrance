using System;
using TourDeFrance.Client.Enums;

namespace TourDeFrance.Client.Player
{
	public class Player
	{
		public Guid Id { get; set; }

		public Guid OwnerId { get; set; }

		public string Nickname { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Gender Gender { get; set; }

		public DateTime? BirthDate { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }
	}

	public class CreateUpdatePlayer
	{
		public string Nickname { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Gender Gender { get; set; }

		public DateTime? BirthDate { get; set; }

		public decimal? Height { get; set; }

		public decimal? Weight { get; set; }
	}
}
