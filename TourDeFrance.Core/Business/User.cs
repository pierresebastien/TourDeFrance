using TourDeFrance.Core.Business.Database;

namespace TourDeFrance.Core.Business
{
	public class AuthenticatedUser : DbUser
	{
		public Client.Responses.AuthenticatedUser ToAuthenticatedModel()
		{
			return ToModel<Client.Responses.AuthenticatedUser>();
		}
	}
}