using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
    public class CreateStageRequestValidator : AbstractValidator<CreateStageRequest>
	{
		public CreateStageRequestValidator()
		{
			RuleFor(x => x.Name).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Name can't be null or empty");
			RuleFor(x => x.Duration).GreaterThan(0).WithMessage("Duration must be strictly positive");
		}
	}

	public class UpdateStageRequestValidator : AbstractValidator<UpdateStageRequest>
	{
		public UpdateStageRequestValidator()
		{
			Include(new ObjectByGuidRequestValidator());
			Include(new CreateStageRequestValidator());
		}
	}
}
