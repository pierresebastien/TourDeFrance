using System;
using SimpleStack.Orm;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Business
{
	public class RaceStage
	{
		public static JoinSqlBuilder<DbRaceStage, DbRaceStage> Sqlbuilder
		{
			get
			{
				return new JoinSqlBuilder<DbRaceStage, DbRaceStage>(Context.Current.DialectProvider)
					.Join<DbRaceStage, DbStage>(x => x.StageId, x => x.Id)
					.Select<DbRaceStage>(x => new {x.Id, x.StageId, x.RaceId, x.Order})
					.Select<DbStage>(x => new {x.Name, x.Duration});
			}
		}

		[Alias("id")]
		public Guid Id { get; set; }

		[Alias("stage_id")]
		public Guid StageId { get; set; }
		
		[Alias("race_id")]
		public Guid RaceId { get; set; }

		[Alias("order")]
		public int Order { get; set; }

		[Alias("name")]
		public string Name { get; set; }

		[Alias("duration")]
		public int Duration { get; set; }

		public Client.Race.RaceStage ToModel()
		{
			return new Client.Race.RaceStage
			{
				Id = Id,
				RaceId = RaceId,
				StageId = StageId,
				Order = Order,
				Duration = Duration,
				RaceName = Name
			};
		}
	}
}