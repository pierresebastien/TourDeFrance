using System;
using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("access_shares")]
	public class DbAccessShare : Auditable
	{
		[ForeignKey(typeof(DbUser))]
		[Alias("sharing_user_id")]
		public Guid SharingUserId { get; set; }

		[ForeignKey(typeof(DbUser))]
		[Alias("shared_user_id")]
		public Guid SharedUserId { get; set; }
	}
}
