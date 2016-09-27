using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;
using TourDeFrance.Client.Responses;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("riders")]
	public class DbRider : BaseOwnObject
	{
		[Alias("first_name")]
		public string FirstName { get; set; }

		[Alias("last_name")]
		public string LastName { get; set; }

		[Alias("gender")]
		public Gender Gender { get; set; }

		[Alias("birth_date")]
		public DateTime? BirthDate { get; set; }

		[Alias("nationality")]
		public string Nationality { get; set; }

		[Alias("height")]
		public decimal? Height { get; set; }

		[Alias("weight")]
		public decimal? Weight { get; set; }

		[Alias("picture")]
		public byte[] Picture { get; set; }

		[ForeignKey(typeof(DbTeam))]
		[Alias("team_id")]
		public Guid TeamId { get; set; }

		public Rider ToModel()
		{
			return new Rider
			{
				Id = Id,
				TeamId = TeamId,
				OwnerId = Owner,
				BirthDate = BirthDate,
				FirstName = FirstName,
				Height = Height,
				LastName = LastName,
				Nationality = Nationality,
				Picture = Picture,
				Weight = Weight
			};
		}
	}
}
