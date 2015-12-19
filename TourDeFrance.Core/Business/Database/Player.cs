using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("players")]
	public class DbPlayer : BaseOwnObject
	{
		[Index(Unique = true)]
		[Alias("nickname")]
		public string Nickname { get; set; }

		[Alias("first_name")]
		public string FirstName { get; set; }

		[Alias("last_name")]
		public string LastName { get; set; }

		[Alias("gender")]
		public Gender Gender { get; set; }

		[Alias("birth_date")]
		public DateTime? BirthDate { get; set; }

		[Alias("height")]
		public decimal? Height { get; set; }

		[Alias("weight")]
		public decimal? Weight { get; set; }

		// TODO: picure ???

		public Client.Player.Player ToModel()
		{
			return new Client.Player.Player
			{
				Id = Id,
				OwnerId = Owner,
				Gender = Gender,
				Height = Height,
				Weight = Weight,
				BirthDate = BirthDate,
				FirstName = FirstName,
				LastName = LastName,
				Nickname = Nickname
			};
		}
	}
}