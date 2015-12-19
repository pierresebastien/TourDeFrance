using System;
using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("drinks")]
	public class DbDrink : BaseOwnObjectNameable
	{
		// TODO: quiche = drink?
		public static readonly Guid QuicheId = new Guid("00000000-0000-0000-0000-000000000001");
		
		[Alias("alcohol_by_volume")]
		// NOTE: in percent
		public decimal? AlcoholByVolume { get; set; }

		[Alias("volume")]
		// NOTE: in centiliter
		public decimal? Volume { get; set; }

		// TODO: formula : 
		// (V x p x e)/(K x m)
		// V = volume de la boisson
		// p = degré d'alcool
		// e = densité éthanol (+/- 0.8)
		// K = coefficient de diffusion (0.7 for men and 0.6 for women)
		// m = masse en kg
	}

	[Alias("sub_drinks")]
	public class DbSubDrink : Auditable
	{
		[ForeignKey(typeof(DbDrink))]
		[Alias("drink_id")]
		public Guid DrinkId { get; set; }

		[ForeignKey(typeof(DbDrink))]
		[Alias("sub_drink_id")]
		public Guid SubDrinkId { get; set; }

		[Alias("volume")]
		public decimal Volume { get; set; }
	}
}