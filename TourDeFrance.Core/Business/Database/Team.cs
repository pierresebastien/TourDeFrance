using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database
{
	[Alias("teams")]
	public class DbTeam : BaseOwnObjectNameable
	{
		// TODO: not used ofr now ???
		[Alias("logo")]
		public byte[] Logo { get; set; }

		public Client.Team.Team ToModel()
		{
			return new Client.Team.Team
			{
				Id = Id,
				Name = Name,
				OwnerId = Owner
			};
		}
	}
}