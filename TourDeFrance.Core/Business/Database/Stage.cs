using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;
using TourDeFrance.Client.Interfaces;
using TourDeFrance.Client.Responses;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("stages")]
	public class DbStage : BaseOwnObjectNameable
	{
		[Alias("duration")]
		// NOTE: in seconds
		public int Duration { get; set; }

		public Stage ToModel()
		{
			return new Stage
			{
				Id = Id,
				Name = Name,
				OwnerId = Owner,
				Duration = Duration
			};
		}
	}

	[Alias("stage_drinks")]
	public class DbStageDrink : Auditable, IOrderable
	{
		[ForeignKey(typeof(DbStage))]
		[Alias("stage_id")]
		public Guid StageId { get; set; }

		[ForeignKey(typeof(DbDrink))]
		[Alias("drink_id")]
		public Guid DrinkId { get; set; }

		[Alias("overrided_volume")]
		// NOTE: in centiliter
		public decimal? OverridedVolume { get; set; }

		[Alias("number_to_drink")]
		public int NumberToDrink { get; set; }

		[Alias("order")]
		public int Order { get; set; }

		[Alias("type")]
		public StageType Type { get; set; }
	}
}