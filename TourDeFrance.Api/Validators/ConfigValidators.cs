using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	public class UpdateConfigRequestValidator : AbstractValidator<UpdateConfigRequest>
	{
		public UpdateConfigRequestValidator()
		{
			Include(new ObjectByIdRequestValidator());
			RuleFor(r => r.Value).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Value cannot be null or empty");
		}
	}
}
