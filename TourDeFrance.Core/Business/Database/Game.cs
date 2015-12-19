using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Enums;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("games")]
	public class DbGame : BaseOwnObject
	{
		[ForeignKey(typeof(DbRace))]
		[Alias("race_id")]
		public Guid RaceId { get; set; }

		[Alias("status")]
		public GameStatus Status { get; set; }

		// NOTE : only players who drink the minimum continue to play
		[Alias("is_strict")]
		public bool IsStrict { get; set; }

		// NOTE : pause between each stages auto
		[Alias("auto_pause")]
		public bool AutoPause { get; set; }
	}

	[Alias("game_participants")]
	public class DbGameParticipant : Auditable
	{
		[ForeignKey(typeof(DbGame))]
		[Alias("game_id")]
		public Guid GameId { get; set; }

		[ForeignKey(typeof (DbPlayer))]
		[Alias("player_id")]
		public Guid? PlayerId { get; set; }

		[ForeignKey(typeof(DbUser))]
		[Alias("user_id")]
		public Guid? UserId { get; set; }

		[ForeignKey(typeof(DbTeam))]
		[Alias("team_id")]
		public Guid TeamId { get; set; }

		[ForeignKey(typeof(DbRider))]
		[Alias("rider_id")]
		public Guid? RiderId { get; set; }
	}
}