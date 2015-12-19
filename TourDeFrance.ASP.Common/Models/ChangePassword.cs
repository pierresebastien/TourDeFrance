using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TourDeFrance.ASP.Common.Models
{
	public class ChangePasswordModel
	{
		[Required]
		public string OldPassword { get; set; }

		[Required]
		public string NewPassword { get; set; }

		[Required]
		[Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
		[DisplayName("Confirmation")]
		public string NewPasswordConfirmation { get; set; }

		public bool IsExpired { get; set; }
	}
}
