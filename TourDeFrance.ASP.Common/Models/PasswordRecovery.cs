using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TourDeFrance.ASP.Common.Models
{
	public class PasswordRecoveryModel
	{
		public string AuthToken { get; set; }

		[Required]
		public string Password { get; set; }

		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		[DisplayName("Confirmation")]
		public string PasswordConfirmation { get; set; }
	}
}
