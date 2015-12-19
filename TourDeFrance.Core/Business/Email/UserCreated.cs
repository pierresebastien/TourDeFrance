namespace TourDeFrance.Core.Business.Email
{
	public class UserCreatedEmailModel : BaseEmailModel
	{
		public string DisplayName { get; set; }

		public string UserName { get; set; }

		public string Password { get; set; }
	}
}
