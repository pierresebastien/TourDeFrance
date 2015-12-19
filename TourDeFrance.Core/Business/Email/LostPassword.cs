using System;

namespace TourDeFrance.Core.Business.Email
{
	public class LostPasswordEmailModel : BaseEmailModel
	{
		public string UserName { get; set; }

		public string LoginName { get; set; }

		public string AuthToken { get; set; }

		public DateTime ExpirationDate { get; set; }
	}
}
