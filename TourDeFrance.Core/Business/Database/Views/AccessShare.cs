using SimpleStack.Orm.Attributes;

namespace TourDeFrance.Core.Business.Database.Views
{
	[Alias("view_share_access")]
	public class ViewAccessShare : DbAccessShare
	{
		[Alias("sharing_first_name")]
		public string SharingFirstName { get; set; }

		[Alias("sharing_last_name")]
		public string SharingLastName { get; set; }

		[Alias("sharing_username")]
		public string SharingUsername { get; set; }

		[Alias("shared_first_name")]
		public string SharedFirstname { get; set; }

		[Alias("shared_last_name")]
		public string SharedLastName { get; set; }

		[Alias("shared_username")]
		public string SharedUsername { get; set; }

		public Client.User.AccessShare ToModel()
		{
			return new Client.User.AccessShare
			{
				Id = Id,
				SharedUsername = SharedUsername,
				SharedFirstname = SharedFirstname,
				SharedLastName = SharedLastName,
				SharedUserId = SharedUserId,
				SharingFirstName = SharingFirstName,
				SharingLastName = SharingLastName,
				SharingUserId = SharingUserId,
				SharingUsername = SharingUsername
			};
		}
	}
}
