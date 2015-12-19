using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("logs")]
	public class DbLog : Auditable
	{
		[ForeignKey(typeof(DbGame))]
		[Alias("game_id")]
		public Guid? GameId { get; set; }

		[ForeignKey(typeof(DbUser))]
		[Alias("user_id")]
		public Guid? UserId { get; set; }

		[ForeignKey(typeof(DbGameParticipant))]
		[Alias("game_participant_id")]
		public Guid? GameParticipantId { get; set; }

		[Alias("date")]
		public DateTime Date { get; set; }

		[Alias("type")]
		public LogType Type { get; set; }

		[Alias("parameters")]
		public string Paramaters { get; set; }
	}
}