using System;
using System.Collections.Generic;
using System.Linq;
using SimpleStack.Orm;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Extensions;

namespace TourDeFrance.Core.Business
{
	[Alias("drinks")]
	public class Drink : DbDrink
	{
		[Ignore]
		public IList<SubDrink> SubDrinks { get; set; }

		[Ignore]
		public bool IsComposedDrink => SubDrinks.Any();

		[Ignore]
		public decimal CalculatedVolume => Volume.CalculateDrinkVolume(SubDrinks);

		[Ignore]
		public decimal CalculatedAlcoholByVolume => AlcoholByVolume.CalculateDrinkAlcoholByVolume(SubDrinks);

		public Client.Responses.Drink ToModel()
		{
			return new Client.Responses.Drink
			{
				Id = Id,
				Name = Name,
				OwnerId = Owner,
				Volume = Volume,
				AlcoholByVolume = AlcoholByVolume,
				IsComposedDrink = IsComposedDrink,
				CalculatedAlcoholByVolume = CalculatedAlcoholByVolume,
				CalculatedVolume = CalculatedVolume,
				SubDrinks = SubDrinks.Select(x => x.ToModel()).ToArray()
			};
		}
	}

	public class SubDrink
	{
		public static JoinSqlBuilder<DbSubDrink, DbSubDrink> Sqlbuilder
		{
			get
			{
				return new JoinSqlBuilder<DbSubDrink, DbSubDrink>(Context.Current.DialectProvider)
					.Join<DbSubDrink, DbDrink>(x => x.SubDrinkId, x => x.Id)
					.Select<DbSubDrink>(x => new {x.Id, x.DrinkId, x.SubDrinkId, x.Volume})
					.Select<DbDrink>(x => new {x.AlcoholByVolume, x.Name});
			}
		}

		[Alias("id")]
		public Guid Id { get; set; }
		
		[Alias("drink_id")]
		public Guid DrinkId { get; set; }
		
		[Alias("sub_drink_id")]
		public Guid SubDrinkId { get; set; }

		[Alias("volume")]
		public decimal Volume { get; set; }

		[Alias("alcohol_by_volume")]
		public decimal? AlcoholByVolume { get; set; }

		[Alias("name")]
		public string Name { get; set; }

		public Client.Responses.SubDrink ToModel()
		{
			return new Client.Responses.SubDrink
			{
				Id = Id,
				DrinkId = SubDrinkId,
				AlcoholByVolume = AlcoholByVolume,
				Name = Name,
				Volume = Volume
			};
		}
	}
}