using System;
using SimpleStack.Orm.Attributes;
using TourDeFrance.Core.Interfaces;

namespace TourDeFrance.Core.Business.Database
{
	public class Auditable : IIdentifiable<Guid>
	{
		[PrimaryKey]
		[Alias("id")]
		public Guid Id { get; set; }

		[Alias("creation_date")]
		public DateTime CreationDate { get; set; }

		[Alias("last_update_date")]
		public DateTime LastUpdateDate { get; set; }

		[Alias("last_update_by")]
		public string LastUpdateBy { get; set; }

		public void BeforeInsert()
		{
			Id = Guid.NewGuid();
			BeforeUpdate();
			CreationDate = LastUpdateDate;
		}

		public void BeforeUpdate()
		{
			LastUpdateDate = DateTime.Now.ToUniversalTime();
			LastUpdateBy = Context.Current.User != null ? Context.Current.User.Username : Constants.SYSTEM_USERNAME;
		}
	}

	public abstract class BaseNameable : Auditable, INameable
	{
		[Index(Unique = true)]
		[Alias("name")]
		public string Name { get; set; }
	}

	public abstract class BaseOwnObject : Auditable, IOwnObject
	{
		[ForeignKey(typeof(DbUser))]
		[Alias("owner")]
		public Guid Owner { get; set; }
	}

	public abstract class BaseOwnObjectNameable : BaseNameable, IOwnObject
	{
		[ForeignKey(typeof(DbUser))]
		[Alias("owner")]
		public Guid Owner { get; set; }
	}
}