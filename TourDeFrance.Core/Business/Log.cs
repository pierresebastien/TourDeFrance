using System;
using SimpleStack.Orm;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Business
{
	public class Log : DbLog
	{
		public static JoinSqlBuilder<DbLog, DbLog> Sqlbuilder
		{
			get
			{
				return new JoinSqlBuilder<DbLog, DbLog>(Context.Current.DialectProvider)
				.LeftJoin<DbLog, DbUser>(x => x.UserId, x => x.Id)
				.LeftJoin<DbLog, DbGameParticipant>(x => x.GameParticipantId, x => x.Id)
				.LeftJoin<DbGameParticipant, DbPlayer>(x => x.PlayerId, x => x.Id)
				.LeftJoin<DbLog, DbGame>(x => x.GameParticipantId, x => x.Id)
				.LeftJoin<DbGame, DbRace>(x => x.RaceId, x => x.Id)
				.SelectAll<DbLog>()
				.Select<DbUser>(x => new { x.Username, UserFirstName = x.FirstName, UserLastName = x.LastName })
				.Select<DbPlayer>(x => new { PlayerId = x.Id, PlayerNickname = x.Nickname, PlayerFirstName = x.FirstName, PlayerLastName = x.LastName })
				.Select<DbRace>(x => new { RaceName = x.Name });
			}
		}

		public string Username { get; set; }

		public string UserFirstName { get; set; }

		public string UserLastName { get; set; }

		[Ignore]
		public string UserDisplayName => UserFirstName + " " + UserLastName;

		public Guid PlayerId { get; set; }

		public string PlayerNickname { get; set; }

		public string PlayerFirstName { get; set; }

		public string PlayerLastName { get; set; }

		[Ignore]
		public string PlayerDisplayName => PlayerFirstName + " " + PlayerLastName;

		public string RaceName { get; set; }
	}
}
