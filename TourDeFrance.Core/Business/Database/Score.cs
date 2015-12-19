using System;
using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("scores")]
	public class DbScore : Auditable
	{
		[ForeignKey(typeof(DbGameParticipant))]
		[Alias("game_participant_id")]
		public Guid GameParticipantId { get; set; }

		[ForeignKey(typeof(DbStage))]
		[Alias("stage_id")]
		public Guid StageId { get; set; }

		[ForeignKey(typeof(DbDrink))]
		[Alias("drink_id")]
		public Guid DrinkId { get; set; }

		[Alias("number")]
		public int Number { get; set; }
	}
}