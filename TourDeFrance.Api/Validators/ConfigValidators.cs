using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	public class UpdateConfigRequestValidator : ObjectByIdRequestValidator<UpdateConfigRequest>
	{
		public UpdateConfigRequestValidator()
		{
			RuleFor(r => r.Value).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Value cannot be null or empty");
		}
	}
}
