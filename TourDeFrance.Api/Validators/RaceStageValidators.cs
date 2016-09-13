using System;
using FluentValidation;
using TourDeFrance.Client.Requests;

namespace TourDeFrance.Api.Validators
{
	public class CreateRaceStageRequestValidator : AbstractValidator<CreateRaceStageRequest>
	{
		public CreateRaceStageRequestValidator()
		{
			RuleFor(r => r.RaceId).Must(x => x == Guid.Empty).WithMessage("Race id cannot be empty");
			RuleFor(r => r.StageId).Must(x => x == Guid.Empty).WithMessage("Stage id cannot be empty");
		}
	}

	public class UpdateRaceStageRequestValidator : AbstractValidator<UpdateRaceStageRequest>
	{
		public UpdateRaceStageRequestValidator()
		{
			Include(new ObjectByGuidRequestValidator());
			RuleFor(x => x.Order).GreaterThan(0).WithMessage("Order must be strictly positive");
		}
	}
}
