using System.Collections.Generic;
using System.Linq;
using TourDeFrance.Core.Business.Database;
using TourDeFrance.Core.Business.Database.Views;

namespace TourDeFrance.Core.Business
{
	public class AuthenticatedUser : DbUser
	{
		public IList<ViewAccessShare> AccessShares { get; set; }

		public Client.Responses.AuthenticatedUser ToAuthenticatedModel()
		{
			Client.Responses.AuthenticatedUser user = ToModel<Client.Responses.AuthenticatedUser>();
			user.AccessShares = AccessShares.Select(x => x.ToModel()).ToArray();
			return user;
		}
	}
}