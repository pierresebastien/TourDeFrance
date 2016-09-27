using SimpleStack.Orm.Attributes;
using TourDeFrance.Client.Responses;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("teams")]
	public class DbTeam : BaseOwnObjectNameable
	{
		// TODO: not used ofr now ???
		[Alias("logo")]
		public byte[] Logo { get; set; }

		public Team ToModel()
		{
			return new Team
			{
				Id = Id,
				Name = Name,
				OwnerId = Owner
			};
		}
	}
}