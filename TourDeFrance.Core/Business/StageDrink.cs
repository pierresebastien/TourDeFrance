using System;
using SimpleStack.Orm;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Business
{
	public class StageDrink
	{
		public static JoinSqlBuilder<DbStageDrink, DbStageDrink> Sqlbuilder
		{
			get
			{
				return new JoinSqlBuilder<DbStageDrink, DbStageDrink>(Context.Current.DialectProvider)
					.Join<DbStageDrink, DbDrink>(x => x.DrinkId, x => x.Id)
					.Select<DbStageDrink>(x => new {x.Id, x.StageId, x.DrinkId, x.OverridedVolume, x.NumberToDrink, x.Order, x.Type})
					.Select<DbDrink>(x => new {x.Name, x.AlcoholByVolume, x.Volume});
			}
		}

		[Alias("id")]
		public Guid Id { get; set; }

		[Alias("stage_id")]
		public Guid StageId { get; set; }
		
		[Alias("drink_id")]
		public Guid DrinkId { get; set; }

		[Alias("overrided_volume")]
		public decimal? OverridedVolume { get; set; }

		[Alias("number_to_drink")]
		public int NumberToDrink { get; set; }

		[Alias("order")]
		public int Order { get; set; }

		[Alias("type")]
		public StageType Type { get; set; }

		[Alias("name")]
		public string Name { get; set; }

		[Alias("alcohol_by_volume")]
		public decimal? AlcoholByVolume { get; set; }

		[Alias("volume")]
		public decimal? Volume { get; set; }

		[Ignore]
		public bool IsComposedDrink { get; set; }

		[Ignore]
		public decimal GameVolume => OverridedVolume ?? Volume ?? 0;

		public Client.Responses.StageDrink ToModel()
		{
			return new Client.Responses.StageDrink
			{
				Id = Id,
				DrinkId = DrinkId,
				DrinkName = Name,
				Volume = Volume,
				AlcoholByVolume = AlcoholByVolume,
				OverridedVolume = OverridedVolume,
				GameVolume = GameVolume,
				NumberToDrink = NumberToDrink,
				Order = Order,
				StageId = StageId,
				Type = Type,
				IsComposedDrink = IsComposedDrink
			};
		}
	}
}